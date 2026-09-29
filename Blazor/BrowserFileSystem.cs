using Microsoft.JSInterop;
using Microsoft.Maui.Storage;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>
/// File system rooted in the in-memory WebAssembly (Emscripten) VFS. Data written to
/// <see cref="AppDataDirectory"/> and <see cref="CacheDirectory"/> does NOT persist
/// across page reloads — use <see cref="Storage.IPreferences"/> or
/// <see cref="Storage.ISecureStorage"/> for durable state.
/// App package files are fetched over HTTP relative to the document base URL
/// (in Blazor WebAssembly, files under wwwroot).
/// </summary>
public class BrowserFileSystem(IJSRuntime js) : IFileSystem
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
		var base64 = await BrowserEssentials.FetchAppFileAsync(js, NormalizePath(filename)).ConfigureAwait(false)
			?? throw new FileNotFoundException($"App package file '{filename}' was not found at the app base URL.", filename);
		return new MemoryStream(Convert.FromBase64String(base64));
	}

	public async Task<bool> AppPackageFileExistsAsync(string filename)
	{
		ArgumentException.ThrowIfNullOrEmpty(filename);
		return await BrowserEssentials.AppFileExistsAsync(js, NormalizePath(filename)).ConfigureAwait(false);
	}

	static string NormalizePath(string filename) => filename.Replace('\\', '/').TrimStart('/');
}
