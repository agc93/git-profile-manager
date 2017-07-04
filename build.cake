#tool "nuget:?package=GitVersion.CommandLine"
#load "build/helpers.cake"
#tool nuget:?package=docfx.console
#addin nuget:?package=Cake.DocFx
#addin nuget:?package=Cake.Docker
#addin nuget:?package=Cake.AzCopy&prerelease

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./src/GitProfileManager.sln");
var projects = GetProjects(solutionPath, configuration);
var artifacts = "./dist/";
var testResultsPath = MakeAbsolute(Directory(artifacts + "./test-results"));
GitVersion versionInfo = null;
var frameworks = new List<string> { "netcoreapp1.1" };
var runtimes = new List<string> { "win10-x64", "osx.10.12-x64", "ubuntu.16.04-x64", "ubuntu.14.04-x64", "centos.7-x64", "debian.8-x64", "rhel.7-x64" };
var PackagedRuntimes = new List<string> { "centos", "ubuntu", "debian", "fedora", "rhel" };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
	versionInfo = GitVersion();
	Information("Building for version {0}", versionInfo.FullSemVer);
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
					ArgumentCustomization = args => args.Append("-r " + runtime),
					Configuration = configuration
				};
				DotNetCorePublish(project.Path.FullPath, settings);
				var publishDir = $"{project.Path.GetDirectory()}/bin/{configuration}/{framework}/{runtime}/publish/";
				CopyDirectory(publishDir, runtimeDir);
				CopyFiles(GetFiles("./build/*.sh"), runtimeDir);
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
	Information("Building packages in new container");
	CreateDirectory($"{artifacts}/packages/");
	foreach(var project in projects.SourceProjects) {
		foreach(var runtime in runtimes.Where(rt => PackagedRuntimes.Any(r => rt.Contains(r)))) {
			var publishDir = $"{artifacts}publish/{project.Name}/{runtime}";
			var sourceDir = MakeAbsolute(Directory(publishDir));
			var packageDir = MakeAbsolute(Directory($"{artifacts}packages/{runtime}"));
			var runSettings = new DockerRunSettings {
				Name = $"docker-fpm-{(runtime.Replace(".", "-"))}",
				Volume = new[] { $"{sourceDir}:/src:ro", $"{packageDir}:/out:rw"},
				Workdir = "/out",
				Rm = true,
				//User = "1000"
			};
			var opts = @"-s dir -a x86_64 --force
			-m 'Alistair Chapman <alistair@agchapman.com>'
			-n 'git-profile-manager'
			--after-install /src/post-install.sh
			--before-remove /src/pre-remove.sh";
			DockerRun(runSettings, "tenzer/fpm", $"{opts} -v {versionInfo.FullSemVer} {GetRuntimeBuild(runtime)} /src/=/usr/lib/git-profile-manager/");
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
			CopyFiles(GetFiles($"./build/{runtime}.nuspec"), publishDir);
			var sourceDir = MakeAbsolute(Directory(publishDir));
			var packageDir = MakeAbsolute(Directory($"{artifacts}packages/{runtime}"));
			var runSettings = new DockerRunSettings {
				Name = $"docker-choco-{(runtime.Replace(".", "-"))}",
				Volume = new[] { 
					$"{sourceDir}:/src/{runtime}:ro",
					$"{packageDir}:/out:rw",
					$"{sourceDir}/{runtime}.nuspec:/src/package.nuspec:ro"
				},
				Workdir = "/src",
				Rm = true
			};
			var opts = @"-y -v
				--outputdirectory /out/
				/src/package.nuspec";
			DockerRun(runSettings, "agc93/mono-choco", $"choco pack --version {versionInfo.NuGetVersionV2} {opts}");
		}
	}
});

Task("Build-Runtime-Package")
	.IsDependentOn("Publish-Runtimes")
	.Does(() => 
{
	Information("Building dotnet package");
	foreach(var project in projects.SourceProjects) {
		CreateDirectory($"{artifacts}packages/dotnet-any");
		Zip($"{artifacts}publish/{project.Name}/dotnet-any/", $"{artifacts}packages/dotnet-any/gpm-dotnet.zip");
	}
});

Task("Build-Docker-Image")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("Build-Linux-Packages")
	.Does(() =>
{
	Information("Building Docker image...");
	CopyFileToDirectory("./build/Dockerfile", artifacts);
	var bSettings = new DockerBuildSettings { Tag = new[] { $"agc93/gpm:{versionInfo.FullSemVer}"}};
	DockerBuild(bSettings, artifacts);
	DeleteFile(artifacts + "Dockerfile");
});

#load "build/publish.cake"
Task("Release")
.IsDependentOn("Publish")
.IsDependentOn("Copy-To-Azure");

Task("Default")
    .IsDependentOn("Post-Build");

Task("Publish")
	.IsDependentOn("Build-Linux-Packages")
	.IsDependentOn("Build-Windows-Packages")
	.IsDependentOn("Build-Runtime-Package")
	.IsDependentOn("Build-Docker-Image")
	.IsDependentOn("Generate-Docs");

RunTarget(target);