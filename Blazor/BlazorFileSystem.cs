using Microsoft.JSInterop;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Avae.Essentials;

public class BlazorFileSystem : IFileSystem
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
        var base64 = await BlazorEssentials.Module.InvokeAsync<string?>("fetchAppFile" ,NormalizePath(filename)).ConfigureAwait(false)
            ?? throw new FileNotFoundException($"App package file '{filename}' was not found at the app base URL.", filename);
        return new MemoryStream(Convert.FromBase64String(base64));
    }

    public async Task<bool> AppPackageFileExistsAsync(string filename)
    {
        ArgumentException.ThrowIfNullOrEmpty(filename);
        return await BlazorEssentials.Module.InvokeAsync<bool>("appFileExists", NormalizePath(filename)).ConfigureAwait(false);
    }

    static string NormalizePath(string filename) => filename.Replace('\\', '/').TrimStart('/');
}
