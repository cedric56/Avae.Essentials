using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Avae.Essentials;

internal class BlazorSms(CircuitServiceAccessor circuitServiceAccessor) : ISms
{
    public bool IsComposeSupported => true;

    public Task ComposeAsync(SmsMessage? message)
    {
        var recipients = string.Join(",", message?.Recipients.Select(Uri.EscapeDataString) ?? []);
        var uri = $"sms:{recipients}";
        if (!string.IsNullOrEmpty(message?.Body))
            uri += "?&body=" + Uri.EscapeDataString(message.Body);

        var launcher =  circuitServiceAccessor.GetRequiredService<ILauncher>();
        return launcher.OpenAsync(new Uri(uri));
    }
}
