using DotNetEnv;

namespace ArthaNiti.Api;

internal static class LocalConfiguration
{
    internal static void Load()
    {
        // Visual Studio and EF tooling can use different working directories.
        var directory = new DirectoryInfo(Path.GetDirectoryName(typeof(LocalConfiguration).Assembly.Location)!);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "ArthaNiti.Api.csproj")))
            directory = directory.Parent;

        var path = Path.Combine(directory?.FullName ?? Directory.GetCurrentDirectory(), ".env");
        if (File.Exists(path))
            Env.NoClobber().Load(path);
    }
}
