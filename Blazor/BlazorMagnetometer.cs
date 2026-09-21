using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices.Sensors;

namespace Avae.Essentials;

internal class BlazorMagnetometer(CircuitServiceAccessor circuitServiceAccessor) : IMagnetometer
{
    BlazorSensors.Magnetometer? magnetometer;

    private void Accelerometer_OnReading(object? sender, EventArgs e)
    {
        if (magnetometer != null)
            ReadingChanged?.Invoke(sender, new MagnetometerChangedEventArgs(
                new MagnetometerData(magnetometer.X, magnetometer.Y, magnetometer.Z)));
    }

    public bool IsSupported => true;

    public bool IsMonitoring => magnetometer?.Activated ?? false;

    public event EventHandler<MagnetometerChangedEventArgs>? ReadingChanged;

    public void Start(SensorSpeed sensorSpeed)
    {
        if (magnetometer == null)
        {
            magnetometer = circuitServiceAccessor.GetRequiredService<BlazorSensors.Magnetometer>();
            magnetometer.OnReading += Accelerometer_OnReading;
        }

        magnetometer.Frequency = sensorSpeed.ToPlatform();
        magnetometer.Start();
    }

    public void Stop()
    {
        magnetometer?.Stop();
    }
}
