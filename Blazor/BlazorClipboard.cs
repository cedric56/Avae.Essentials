using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace Avae.Essentials;

/// <summary>
/// Clipboard backed by the async Clipboard API (navigator.clipboard).
/// Browsers gate clipboard reads behind a user permission prompt, and there is no
/// synchronous "has text" query — <see cref="HasText"/> reflects the last value
/// observed through this API rather than the live OS clipboard.
/// </summary>
public class BlazorClipboard : IClipboard
{
    bool lastKnownHasText;

    public bool HasText => lastKnownHasText;

    public event EventHandler<EventArgs>? ClipboardContentChanged;

    public async Task SetTextAsync(string? text)
    {
        await BlazorEssentials.Module.InvokeVoidAsync("clipboardWriteText", text ?? string.Empty).ConfigureAwait(false);
        lastKnownHasText = !string.IsNullOrEmpty(text);
        ClipboardContentChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<string?> GetTextAsync()
    {
        var text = await BlazorEssentials.Module.InvokeAsync<string?>("clipboardReadText").ConfigureAwait(false);
        lastKnownHasText = !string.IsNullOrEmpty(text);
        return string.IsNullOrEmpty(text) ? null : text;
    }
}
