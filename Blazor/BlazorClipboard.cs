using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using PatrickJahr.Blazor.AsyncClipboard;

namespace Avae.Essentials;

internal class BlazorClipboard(CircuitServiceAccessor circuitServiceAccessor) : IClipboard
{
    public bool HasText => throw new NotImplementedException();

    public event EventHandler<EventArgs>? ClipboardContentChanged;

    public async Task<string?> GetTextAsync()
    {
        var service = circuitServiceAccessor.GetRequiredService<AsyncClipboardService>();
        return await service.ReadTextAsync();
    }

    public async Task SetTextAsync(string? text)
    {
        var service = circuitServiceAccessor.GetRequiredService<AsyncClipboardService>();
        ClipboardContentChanged?.Invoke(this, EventArgs.Empty);
        await service.WriteTextAsync(text ?? string.Empty);
    }
}
