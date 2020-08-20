Task("Copy-To-Azure")
.IsDependentOn("Publish")
.Does(() => {
    Information("Uploading packages for {0} using AzCopy...", packageVersion);
    /* AzCopy($"{artifacts}packages/", $"https://appstored.blob.core.windows.net/gpm/{packageVersion}", settings => {
        settings.CopyRecursively()
            .UseDestinationAccountKey(EnvironmentVariable("AZURE_STORAGE_KEY"));
    });
    Information("Uploading packages for {0} using AzCopy...", "latest");
    AzCopy($"{artifacts}packages/", $"https://appstored.blob.core.windows.net/gpm/latest", settings => {
        settings.CopyRecursively()
            .UseDestinationAccountKey(EnvironmentVariable("AZURE_STORAGE_KEY"));
    }); */
});

Task("Build-Archives")
.IsDependentOn("Build-Warp-Package")
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
.WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("NUGET_TOKEN")))
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