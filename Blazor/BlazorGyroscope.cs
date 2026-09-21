using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices.Sensors;

namespace Avae.Essentials;

internal class BlazorGyroscope(CircuitServiceAccessor circuitServiceAccessor) : IGyroscope
{
    BlazorSensors.Gyroscope? gyroscope;

    private void Accelerometer_OnReading(object? sender, EventArgs e)
    {
        if (gyroscope != null)
            ReadingChanged?.Invoke(sender, new GyroscopeChangedEventArgs(
                new GyroscopeData(gyroscope.X, gyroscope.Y, gyroscope.Z)));
    }

    public bool IsSupported => true;

    public bool IsMonitoring => gyroscope?.Activated ?? false;

    public event EventHandler<GyroscopeChangedEventArgs>? ReadingChanged;

    public void Start(SensorSpeed sensorSpeed)
    {
        if (gyroscope == null)
        {
            gyroscope = circuitServiceAccessor.GetRequiredService<BlazorSensors.Gyroscope>();
            gyroscope.OnReading += Accelerometer_OnReading;
        }

        gyroscope.Frequency = sensorSpeed.ToPlatform();
        gyroscope.Start();
    }

    public void Stop()
    {
        gyroscope?.Stop();
    }
}
