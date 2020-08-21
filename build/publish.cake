Task("Build-Archives")
.IsDependentOn("Build-Warp-Package")
.WithCriteria(() => HasEnvironmentVariable("GITHUB_REF"))
.WithCriteria(() => EnvironmentVariable("GITHUB_REF").StartsWith("refs/tags/v"))
.Does(() => {
    var warpDir = $"{artifacts}warp/";
    CreateDirectory($"{artifacts}archive");
    foreach (var file in GetFiles(warpDir + "**/*"))
    {
        var dir = file.GetDirectory();
        Information("Building archive for {0}", dir);
        Zip(dir, $"{artifacts}archive/gpm-{dir.GetDirectoryName()}.zip");
    }
});

Task("Publish-NuGet-Package")
.IsDependentOn("Build-NuGet-Package")
.WithCriteria(() => HasEnvironmentVariable("NUGET_TOKEN"))
.WithCriteria(() => EnvironmentVariable("GITHUB_REF").StartsWith("refs/tags/v"))
.Does(() => {
    var nupkgDir = $"{artifacts}packages";
    var nugetToken = EnvironmentVariable("NUGET_TOKEN");
    var pkgFiles = GetFiles($"{nupkgDir}/dotnet-any/*.nupkg");
    NuGetPush(pkgFiles, new NuGetPushSettings {
      Source = "https://api.nuget.org/v3/index.json",
      ApiKey = nugetToken
    });
});

/*Task("Build-Downlink-Image")
    .IsDependentOn("Publish")
    .Does(() => 
{
    CopyFile("./build/Dockerfile.downlink", $"{artifacts}packages/Dockerfile");
    var buildSettings = new DockerBuildSettings { Tag = new[] { $"agc93/gpm-downlink:{versionInfo.FullSemVer}"}};
    DockerBuild(buildSettings, $"{artifacts}packages");
    DeleteFile($"{artifacts}packages/Dockerfile");
}); */