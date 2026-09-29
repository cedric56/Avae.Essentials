using Microsoft.JSInterop;
using Microsoft.Maui.Accessibility;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>Screen reader announcements via a visually-hidden aria-live region.</summary>
public class BrowserSemanticScreenReader(IJSRuntime js) : ISemanticScreenReader
{
    public async void Announce(string text)
	{
		BrowserEssentials.EnsureInitialized();
		await BrowserEssentials.AnnounceAsync(js, text);
	}
}
