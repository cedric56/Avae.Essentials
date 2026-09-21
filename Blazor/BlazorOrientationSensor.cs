using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices.Sensors;

namespace Avae.Essentials;

internal class BlazorOrientationSensor(CircuitServiceAccessor circuitServiceAccessor) : IOrientationSensor
{
    BlazorSensors.AbsoluteOrientationSensor? sensor;

    private void Accelerometer_OnReading(object? sender, EventArgs e)
    {
        if (sensor != null)

            ReadingChanged?.Invoke(sender, new OrientationSensorChangedEventArgs(
                new OrientationSensorData(sensor.X, sensor.Y, sensor.Z, sensor.W)));
    }

    public bool IsSupported => true;

    public bool IsMonitoring => sensor?.Activated ?? false;

    public event EventHandler<OrientationSensorChangedEventArgs>? ReadingChanged;

    public void Start(SensorSpeed sensorSpeed)
    {
        if (sensor == null)
        {
            sensor = circuitServiceAccessor.GetRequiredService<BlazorSensors.AbsoluteOrientationSensor>();
            sensor.OnReading += Accelerometer_OnReading;
        }

        sensor.Frequency = sensorSpeed.ToPlatform();
        sensor.Start();
    }

    public void Stop()
    {
        sensor?.Stop();
    }
}
