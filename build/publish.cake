Task("Copy-To-Azure")
.IsDependentOn("Publish")
.Does(() => {
    Information("Uploading packages for {0} using AzCopy...", versionInfo.FullSemVer);
    AzCopy($"{artifacts}packages/", $"https://appstored.blob.core.windows.net/gpm/{versionInfo.FullSemVer}", settings => {
        settings.CopyRecursively()
            .UseDestinationAccountKey(EnvironmentVariable("AZURE_STORAGE_KEY"));
    });
    Information("Uploading packages for {0} using AzCopy...", "latest");
    AzCopy($"{artifacts}packages/", $"https://appstored.blob.core.windows.net/gpm/latest", settings => {
        settings.CopyRecursively()
            .UseDestinationAccountKey(EnvironmentVariable("AZURE_STORAGE_KEY"));
    });
});

Task("Publish-To-GitHub")
.IsDependentOn("Publish")
.Does(() => {
    Information("Publishing to GitHub...");
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