using Microsoft.JSInterop;
using Microsoft.Maui.Accessibility;

namespace Avae.Essentials;

/// <summary>Screen reader announcements via a visually-hidden aria-live region.</summary>
public class BlazorSemanticScreenReader : ISemanticScreenReader
{
    public async void Announce(string text)
    {
        BlazorEssentials.EnsureInitialized();
        await BlazorEssentials.Module.InvokeVoidAsync("announce", text);
    }
}
