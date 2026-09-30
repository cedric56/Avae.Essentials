using Microsoft.JSInterop;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

public class BlazorFileSystem(IJSRuntime js) : IFileSystem
{
    public string CacheDirectory => EnsureDirectory("/cache");

    public string AppDataDirectory => EnsureDirectory("/appdata");

    static string EnsureDirectory(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }

    public async Task<Stream> OpenAppPackageFileAsync(string filename)
    {
        ArgumentException.ThrowIfNullOrEmpty(filename);
        var base64 = await BlazorEssentialsInterop.InvokeWithRetryAsync<string?>(js, "fetchAppFile" ,NormalizePath(filename)).ConfigureAwait(false)
            ?? throw new FileNotFoundException($"App package file '{filename}' was not found at the app base URL.", filename);
        return new MemoryStream(Convert.FromBase64String(base64));
    }

    public async Task<bool> AppPackageFileExistsAsync(string filename)
    {
        ArgumentException.ThrowIfNullOrEmpty(filename);
        return await BlazorEssentialsInterop.InvokeWithRetryAsync<bool>(js, "appFileExists", NormalizePath(filename)).ConfigureAwait(false);
    }

    static string NormalizePath(string filename) => filename.Replace('\\', '/').TrimStart('/');
}
