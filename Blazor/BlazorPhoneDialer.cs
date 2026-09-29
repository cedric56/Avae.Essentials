using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Avae.Essentials;


/// <summary>Phone dialing via a tel: link (effective on mobile browsers or with a desktop handler).</summary>
public class BlazorPhoneDialer(IJSRuntime js) : IPhoneDialer
{
    public bool IsSupported => true;

    public async void Open(string number)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        BlazorEssentials.EnsureInitialized();
        await BlazorLauncher.NavigateToAsync(js, "tel:" + Uri.EscapeDataString(number));
    }
}
