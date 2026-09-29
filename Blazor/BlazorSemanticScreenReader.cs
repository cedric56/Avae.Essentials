using Microsoft.JSInterop;
using Microsoft.Maui.Accessibility;

namespace Avae.Essentials;

/// <summary>Screen reader announcements via a visually-hidden aria-live region.</summary>
public class BlazorSemanticScreenReader(IJSRuntime js) : ISemanticScreenReader
{
    public async void Announce(string text)
    {
        BlazorEssentials.EnsureInitialized();
        await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "announce", text);
    }
}
