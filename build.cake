#tool "nuget:?package=GitVersion.CommandLine"
#load "build/helpers.cake"
#tool nuget:?package=docfx.console
#addin nuget:?package=Cake.DocFx
#addin nuget:?package=Cake.Docker
//#addin nuget:?package=Cake.AzCopy&version=0.1.1

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var fallbackVersion = Argument<string>("force-version", EnvironmentVariable("FALLBACK_VERSION") ?? "0.2.0");

///////////////////////////////////////////////////////////////////////////////
// VERSIONING
///////////////////////////////////////////////////////////////////////////////

var packageVersion = string.Empty;
#load "build/version.cake"

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./src/GitProfileManager.sln");
var projects = GetProjects(solutionPath, configuration);
var artifacts = "./dist/";
var testResultsPath = MakeAbsolute(Directory(artifacts + "./test-results"));
var frameworks = new List<string> { "netcoreapp3.1" };
var runtimes = new List<string> { "win-x64", "osx-x64", "linux-x64" };
// var PackagedRuntimes = new List<string> { "centos", "ubuntu", "debian", "fedora", "rhel" };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
	packageVersion = BuildVersion(fallbackVersion);
	if (FileExists("./build/.dotnet/dotnet.exe")) {
		Information("Using local install of `dotnet` SDK!");
		Context.Tools.RegisterFile("./build/.dotnet/dotnet.exe");
	}
	Verbose("Building for " + string.Join(", ", frameworks));
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in projects.AllProjectPaths)
	{
		Information("Cleaning {0}", path);
		CleanDirectories(path + "/**/bin/" + configuration);
		CleanDirectories(path + "/**/obj/" + configuration);
	}
	Information("Cleaning common files...");
	CleanDirectory(artifacts);
});

Task("Restore")
	.Does(() =>
{
	// Restore all NuGet packages.
	Information("Restoring solution...");
	foreach (var project in projects.AllProjectPaths) {
		DotNetCoreRestore(project.FullPath);
	}
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	Information("Building solution...");
	foreach(var framework in frameworks) {
		foreach (var project in projects.SourceProjectPaths) {
			var settings = new DotNetCoreBuildSettings {
				Framework = framework,
				Configuration = configuration,
				NoIncremental = true,
			};
			DotNetCoreBuild(project.FullPath, settings);
		}
	}
	
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
    CreateDirectory(testResultsPath);
	if (projects.TestProjects.Any()) {

		var settings = new DotNetCoreTestSettings {
			Configuration = configuration
		};

		foreach(var project in projects.TestProjects) {
			DotNetCoreTest(project.Path.FullPath, settings);
		}
	}
});

Task("Generate-Docs")
.IsDependentOn("Post-Build") //since we're now using assemblies because docfx broke Linux compat
	.Does(() => 
{
	DocFxMetadata("./docfx/docfx.json");
	DocFxBuild("./docfx/docfx.json");
	Zip("./docfx/_site/", artifacts + "/docfx.zip");
})
.OnError(ex => 
{
	Warning("Error generating documentation!");
});

Task("Post-Build")
	.IsDependentOn("Build")
	.IsDependentOn("Run-Unit-Tests")
	.Does(() =>
{
	CreateDirectory(artifacts + "build");
	foreach (var project in projects.SourceProjects) {
		CreateDirectory(artifacts + "build/" + project.Name);
		foreach (var framework in frameworks) {
			var frameworkDir = $"{artifacts}build/{project.Name}/{framework}";
			CreateDirectory(frameworkDir);
			var files = GetFiles($"{project.Path.GetDirectory()}/bin/{configuration}/{framework}/*.*");
			CopyFiles(files, frameworkDir);
		}
	}
});

Task("Publish-Runtimes")
	.IsDependentOn("Post-Build")
	.Does(() =>
{
	CreateDirectory(artifacts + "publish/");
	foreach (var project in projects.SourceProjects) {
		foreach (var framework in frameworks) {
			var projectDir = $"{artifacts}publish/{project.Name}";
			CreateDirectory(projectDir);
			// runtime (native) publish
			foreach(var runtime in runtimes) {
				var runtimeDir = $"{projectDir}/{runtime}";
				CreateDirectory(runtimeDir);
				Information("Publishing {0} for {1} runtime", project.Name, runtime);
				var rSettings = new DotNetCoreRestoreSettings {
					ArgumentCustomization = args => args.Append("-r " + runtime)
				};
				DotNetCoreRestore(project.Path.FullPath, rSettings);
				var settings = new DotNetCorePublishSettings {
					ArgumentCustomization = args => args.Append("-r " + runtime).Append("/p:PackAsTool=false"),
					PublishSingleFile = true,
					PublishTrimmed = true,
					Configuration = configuration
				};
				DotNetCorePublish(project.Path.FullPath, settings);
				var publishDir = $"{project.Path.GetDirectory()}/bin/{configuration}/{framework}/{runtime}/publish/";
				CopyDirectory(publishDir, runtimeDir);
				CopyFiles(GetFiles("./scripts/*.sh"), runtimeDir);
				CopyFiles(GetFiles("./scripts/*.ps1"), runtimeDir);
			}
			// platform (dotnet) publish
			var pDir = MakeAbsolute(Directory($"{projectDir}/dotnet-any/"));
			Information("Publishing {0} for {1} runtime", project.Name, "dotnet");
			var pSettings = new DotNetCorePublishSettings {
				ArgumentCustomization = args => args.Append($"-o {pDir}"),
				Configuration = configuration
			};
			DotNetCorePublish(project.Path.FullPath, pSettings);
		}
	}
});

Task("Build-Linux-Packages")
	.IsDependentOn("Publish-Runtimes")
	.WithCriteria(IsRunningOnUnix())
	.Does(() => 
{
	Information("Building packages in fpm containers");
	CreateDirectory($"{artifacts}/packages/");
	foreach(var project in projects.SourceProjects) {
		Information("Packaging {0}", project.Name);
		foreach(var runtimeRef in Runtimes) {
			var runtime = runtimeRef.Key;
			// var publishDir = $"{artifacts}publish/{project.Name}/{runtime}";
			var publishDir = $"{artifacts}publish/{project.Name}/linux-x64";
			var sourceDir = MakeAbsolute(Directory(publishDir));
			var packageDir = MakeAbsolute(Directory($"{artifacts}packages/{runtime}"));
			var runSettings = new DockerContainerRunSettings {
				Name = $"docker-fpm-{(runtime.Replace(".", "-"))}",
				Volume = new[] { $"{sourceDir}:/src:ro", $"{packageDir}:/out:rw"},
				Workdir = "/out",
				Rm = true,
				//User = "1000"
			};
			var opts = string.Join(" ", new List<string> {
				"-s dir -a x86_64 --force",
				"-m \"Alistair Chapman <alistair@agchapman.com>\"",
				"-n git-profile-manager",
				"--after-install /src/post-install.sh",
				"--before-remove /src/pre-remove.sh"
			});
			Information("Starting {0} for {1} ({2})", runSettings.Name, runtime, "linux-x64");
			DockerRun(runSettings, "tenzer/fpm", $"{opts} -v {packageVersion} {runtimeRef.Value} /src/=/usr/lib/git-profile-manager/");
		}
	}
});

Task("Build-Windows-Packages")
	.IsDependentOn("Publish-Runtimes")
	.WithCriteria(IsRunningOnUnix())
	.Does(() => 
{
	Information("Building Chocolatey package in new container");
	CreateDirectory($"{artifacts}packages");
	foreach(var project in projects.SourceProjects) {
		foreach(var runtime in runtimes.Where(r => r.StartsWith("win"))) {
			var publishDir = $"{artifacts}publish/{project.Name}/{runtime}";
			CopyFiles(GetFiles($"./build/{runtime}.nuspec") + GetFiles("./build/choco/*"), publishDir);
			var sourceDir = MakeAbsolute(Directory(publishDir));
			var packageDir = MakeAbsolute(Directory($"{artifacts}packages/{runtime}"));
			var runSettings = new DockerContainerRunSettings {
				Name = $"docker-choco-{(runtime.Replace(".", "-"))}",
				Volume = new[] { 
					$"{sourceDir}:/src/{runtime}:ro",
					$"{packageDir}:/out:rw",
					$"{sourceDir}/{runtime}.nuspec:/src/package.nuspec:ro"
				},
				Workdir = "/src",
				Rm = true
			};
			var opts = @"-y -v --outputdirectory /out/ /src/package.nuspec";
			DockerRun(runSettings, "agc93/mono-choco", $"choco pack --version {packageVersion} {opts}");
		}
	}
});

Task("Build-NuGet-Package")
	.IsDependentOn("Publish-Runtimes")
	.Does(() => 
{
	Information("Building dotnet package");
	foreach(var project in projects.SourceProjects) {
		CreateDirectory($"{artifacts}packages/dotnet-any");
		var packSettings = new DotNetCorePackSettings {
			ArgumentCustomization = args => args.Append($"/p:Version={packageVersion}"),
			Configuration = configuration,
			OutputDirectory = $"{artifacts}packages/dotnet-any/"
		};
		DotNetCorePack(project.Path.FullPath, packSettings);
		// Zip($"{artifacts}publish/{project.Name}/dotnet-any/", $"{artifacts}packages/dotnet-any/gpm-dotnet.zip");
	}
});

#load "build/warp.cake"

Task("Build-Warp-Package")
	.IsDependentOn("Publish-Runtimes")
	.Does(() =>
{
	Information("Building Warp packages");
	CreateDirectory($"{artifacts}warp");
	foreach(var runtime in runtimes) {
		CreateDirectory($"{artifacts}warp/{runtime}");
		Warp($"./dist/publish/GitProfileManager/{runtime}",
			$"git-profile-manager{(runtime.StartsWith("win") ? ".exe" : string.Empty)}",
			$"{artifacts}warp/{runtime}/gpm{(runtime.StartsWith("win") ? ".exe" : string.Empty)}",
			GetWarpPlatform(runtime));
	}
});


#load "build/publish.cake"
Task("Release")
.IsDependentOn("Publish")
.IsDependentOn("Publish-NuGet-Package");

Task("Default")
    .IsDependentOn("Post-Build");

Task("Package")
	.IsDependentOn("Build-Linux-Packages")
	.IsDependentOn("Build-Windows-Packages")
	.IsDependentOn("Build-NuGet-Package");

Task("Publish")
	.IsDependentOn("Build-Linux-Packages")
	.IsDependentOn("Build-Windows-Packages")
	.IsDependentOn("Build-NuGet-Package")
	// .IsDependentOn("Build-Docker-Image")
	// .IsDependentOn("Generate-Docs")
	.IsDependentOn("Build-Archives")
	.IsDependentOn("Build-Warp-Package");

RunTarget(target);