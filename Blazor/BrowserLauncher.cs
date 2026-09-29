using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>
/// Launcher backed by window.open for web URLs and location.assign for
/// protocol-handler schemes (mailto:, tel:, sms:). Files open as blob URLs in a new tab.
/// </summary>
public class BrowserLauncher(IJSRuntime js) : ILauncher
{
	static readonly string[] NavigationSchemes = ["mailto", "tel", "sms"];

	public Task<bool> CanOpenAsync(Uri uri) =>
		Task.FromResult(uri.Scheme is "http" or "https" || NavigationSchemes.Contains(uri.Scheme));

    public async Task<bool> OpenAsync(Uri uri)
	{
		return NavigationSchemes.Contains(uri.Scheme)
			? await BrowserEssentials.NavigateToAsync(js, uri.AbsoluteUri)
			: await BrowserEssentials.OpenUrlAsync(js, uri.AbsoluteUri);
	}

	public async Task<bool> OpenAsync(OpenFileRequest request)
	{
		if (request.File is null)
			return false;
		var bytes = await File.ReadAllBytesAsync(request.File.FullPath).ConfigureAwait(false);
		return await BrowserEssentials.OpenFileBlobAsync(js,
            Convert.ToBase64String(bytes),
			request.File.ContentType,
			Path.GetFileName(request.File.FullPath));
	}

	public async Task<bool> TryOpenAsync(Uri uri) =>
		await CanOpenAsync(uri).ConfigureAwait(false) && await OpenAsync(uri).ConfigureAwait(false);
}
