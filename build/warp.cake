#addin nuget:?package=Cake.Warp&version=0.1.0

public WarpPlatforms GetWarpPlatform(string runtime) {
    var warp = new Dictionary<string, WarpPlatforms> {
        ["osx-x64"] = WarpPlatforms.MacOSX64,
        ["win-x64"] = WarpPlatforms.WindowsX64,
        ["linux-x64"] = WarpPlatforms.LinuxX64
    };
    var s = warp[runtime];
    return s;
}