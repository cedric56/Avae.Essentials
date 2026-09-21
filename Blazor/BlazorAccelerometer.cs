using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices.Sensors;

namespace Avae.Essentials;

internal static partial class SensorSpeedExtensions
{
    internal static double ToPlatform(this SensorSpeed sensorSpeed)
    {
        switch (sensorSpeed)
        {
            case SensorSpeed.Fastest:
                return sensorIntervalFastest;
            case SensorSpeed.Game:
                return sensorIntervalGame;
            case SensorSpeed.UI:
                return sensorIntervalUI;
        }

        return sensorIntervalDefault;
    }
}
internal static partial class SensorSpeedExtensions
{
    // Timing intervals to match Android sensor speeds in milliseconds
    // https://developer.android.com/guide/topics/sensors/sensors_overview
    internal const double sensorIntervalDefault = 200;
    internal const double sensorIntervalUI = 60;
    internal const double sensorIntervalGame = 20;
    internal const double sensorIntervalFastest = 5;
}

internal class BlazorAccelerometer(CircuitServiceAccessor circuitServiceAccessor) : IAccelerometer
{
    BlazorSensors.Accelerometer? accelerometer;

    private void Accelerometer_OnReading(object? sender, EventArgs e)
    {
        if (accelerometer != null)
            ReadingChanged?.Invoke(sender, new AccelerometerChangedEventArgs(
                new AccelerometerData(accelerometer.X, accelerometer.Y, accelerometer.Z)));
    }

    public bool IsSupported => true;

    public bool IsMonitoring => accelerometer?.Activated ?? false;

    public event EventHandler<AccelerometerChangedEventArgs>? ReadingChanged;

    public event EventHandler? ShakeDetected;

    public void Start(SensorSpeed sensorSpeed)
    {
        if (accelerometer == null)
        {
            accelerometer = circuitServiceAccessor.GetRequiredService<BlazorSensors.Accelerometer>();
            accelerometer.OnReading += Accelerometer_OnReading;
        }

        accelerometer.Frequency = sensorSpeed.ToPlatform();
        accelerometer.Start();
    }

    public void Stop()
    {
        accelerometer?.Stop();
    }
}
