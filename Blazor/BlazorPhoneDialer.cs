using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Avae.Essentials;

internal class BlazorPhoneDialer(CircuitServiceAccessor circuitServiceAccessor) : IPhoneDialer
{
    public bool IsSupported => true;

    public async void Open(string number)
    {
        var launcher = circuitServiceAccessor.GetRequiredService<ILauncher>();
        await launcher.OpenAsync(new Uri($"tel:{number}"));
    }
}
