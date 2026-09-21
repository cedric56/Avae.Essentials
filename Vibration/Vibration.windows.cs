using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using System.Runtime.Versioning;
using Windows.Devices.Haptics;
using Windows.Foundation.Metadata;

namespace Avae.Essentials;

partial class VibrationImplementation : IVibration
{
    public void Vibrate()
    {
        if (!IsSupported)
            throw new FeatureNotSupportedException();

        PlatformVibrate();
    }

    public void Vibrate(TimeSpan duration)
    {
        if (!IsSupported)
            throw new FeatureNotSupportedException();

        if (duration.TotalMilliseconds < 0)
            duration = TimeSpan.Zero;
        else if (duration.TotalSeconds > 5)
            duration = TimeSpan.FromSeconds(5);

        PlatformVibrate(duration);
    }

    public void Cancel()
    {
        if (!IsSupported)
            throw new FeatureNotSupportedException();

        PlatformCancel();
    }
}

[SupportedOSPlatform("windows10.0.10240")]
partial class VibrationImplementation : IVibration
{
    public bool IsSupported
        => ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification.VibrationDevice") && DefaultDevice != null;

    static VibrationDevice DefaultDevice =>
        throw new NotImplementedException("WINUI"); //VibrationDevice.GetDefault();

    void PlatformVibrate()
        => throw new NotImplementedException("WINUI");// DefaultDevice.Vibrate(duration);

    void PlatformVibrate(TimeSpan duration) =>
        throw new NotImplementedException("WINUI");// DefaultDevice.Vibrate(duration);

    void PlatformCancel() =>
        throw new NotImplementedException("WINUI");//DefaultDevice.Cancel();
}
