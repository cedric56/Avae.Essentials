using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;

namespace Avae.Essentials;
/// <summary>
/// Launcher backed by window.open for web URLs and location.assign for
/// protocol-handler schemes (mailto:, tel:, sms:). Files open as blob URLs in a new tab.
/// </summary>
public class BlazorLauncher(IJSRuntime js) : ILauncher
{
    internal static async Task<bool> OpenUrlAsync(IJSRuntime js, string url)
    => await BlazorEssentialsInterop.InvokeWithRetryAsync<bool>(js, "openUrl", url);

    internal static async Task<bool> NavigateToAsync(IJSRuntime js, string url)
     => await BlazorEssentialsInterop.InvokeWithRetryAsync<bool>(js, "navigateTo", url);

    internal static async Task<bool> OpenFileBlobAsync(IJSRuntime js, string base64, string? contentType, string name)
    => await BlazorEssentialsInterop.InvokeWithRetryAsync<bool>(js, "openFileBlob", base64, contentType, name);


    static readonly string[] NavigationSchemes = ["mailto", "tel", "sms"];

    public Task<bool> CanOpenAsync(Uri uri) =>
        Task.FromResult(uri.Scheme is "http" or "https" || NavigationSchemes.Contains(uri.Scheme));

    public async Task<bool> OpenAsync(Uri uri)
    {
        return NavigationSchemes.Contains(uri.Scheme)
            ? await NavigateToAsync(js, uri.AbsoluteUri)
            : await OpenUrlAsync(js, uri.AbsoluteUri);
    }

    public async Task<bool> OpenAsync(OpenFileRequest request)
    {
        if (request.File is null)
            return false;
        var bytes = await File.ReadAllBytesAsync(request.File.FullPath).ConfigureAwait(false);
        return await OpenFileBlobAsync(js,
            Convert.ToBase64String(bytes),
            request.File.ContentType,
            Path.GetFileName(request.File.FullPath));
    }

    public async Task<bool> TryOpenAsync(Uri uri) =>
        await CanOpenAsync(uri).ConfigureAwait(false) && await OpenAsync(uri).ConfigureAwait(false);
}

