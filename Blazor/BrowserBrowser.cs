using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>Opens URIs in a new browser tab via window.open.</summary>
public class BrowserBrowser(IJSRuntime js) : IBrowser
{
    public async Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options)
	{
		// Launch mode and title/color options have no browser equivalent — always a new tab.
		return await BrowserEssentials.OpenUrlAsync(js, uri.AbsoluteUri);
	}
}
