using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using System;

namespace Avae.Essentials;

/// <summary>The Magnetometer sensor API is not broadly available to web content.</summary>
public class BlazorMagnetometer : IMagnetometer
{
    public bool IsSupported => false;

    public bool IsMonitoring => false;

    public event EventHandler<MagnetometerChangedEventArgs>? ReadingChanged
    {
        add { }
        remove { }
    }

    public void Start(SensorSpeed sensorSpeed) =>
        throw new FeatureNotSupportedException("The magnetometer is not available in the browser.");

    public void Stop()
    {
    }
}
