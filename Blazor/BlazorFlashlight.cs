using Microsoft.JSInterop;
using Microsoft.Maui.Devices;
using System.Threading.Tasks;

namespace Avae.Essentials;

/// <summary>The camera torch is not reachable from the browser.</summary>
public class BlazorFlashlight : IFlashlight
{
    public async Task<bool> IsSupportedAsync() =>
        await BlazorEssentials.Module.InvokeAsync<bool>("isTorchSupported");
    
    public async Task TurnOnAsync() =>
        await BlazorEssentials.Module.InvokeVoidAsync("on");

    public async Task TurnOffAsync() =>
        await BlazorEssentials.Module.InvokeVoidAsync("off");
}
