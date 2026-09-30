using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.Json;

namespace Avae.Essentials;

/// <summary>
/// Shared plumbing for sensors backed by devicemotion/deviceorientation events.
/// SensorSpeed is ignored — browsers deliver events at their own fixed rate.
/// On iOS Safari the first Start triggers the motion-permission prompt and must be
/// called from a user gesture.
/// </summary>
public abstract class BlazorSensorBase(IJSRuntime js, string kind)
{
    private readonly string _kind = kind;

    private DotNetObjectReference<BlazorSensorBase>? _ref;
    private bool _isSupported;

    public bool IsSupported
    {
        get
        {
            return _isSupported;
        }
    }

    public bool IsMonitoring { get; private set; }

    /// <summary>Imports the module and checks browser support. Idempotent.</summary>
    public async Task InitializeAsync()
    {
        _isSupported = await BlazorEssentialsInterop.InvokeWithRetryAsync<bool>(js, "sensorIsSupported", _kind);
    }

    private protected async Task StartCoreAsync(SensorSpeed sensorSpeed)
    {
        if (!IsSupported)
            throw new FeatureNotSupportedException($"The {_kind} sensor is not available in this browser.");
        if (IsMonitoring)
            throw new InvalidOperationException($"The {_kind} sensor is already being monitored.");

        _ref = DotNetObjectReference.Create(this);
        var frequencyHz = ToFrequencyHz(sensorSpeed);

        bool started;
        try
        {
            started = await BlazorEssentialsInterop.InvokeWithRetryAsync<bool>(js, "sensorStart", _ref, _kind, frequencyHz);
        }
        catch (JSDisconnectedException)
        {
            started = false;
        }

        IsMonitoring = started;
        if (!started)
        {
            _ref?.Dispose();
            _ref = null;
        }
    }

    private static double ToFrequencyHz(SensorSpeed speed) => speed switch
    {
        SensorSpeed.Fastest => 60,
        SensorSpeed.Game => 30,
        SensorSpeed.UI => 15,
        SensorSpeed.Default or _ => 5,
    };

    public async void Stop()
    {
        if (!IsMonitoring)
            return;

        IsMonitoring = false;
        try
        {
            await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "sensorStop", _kind);
        }
        finally
        {
            _ref?.Dispose();
            _ref = null;
        }
    }

    [JSInvokable]
    public void OnReading(string json) => OnReadingCore(json);

    [JSInvokable]
    public void OnError(string message) => OnErrorCore(message);

    private protected abstract void OnReadingCore(string json);

    /// <summary>Override to surface sensor errors (default: ignored).</summary>
    private protected virtual void OnErrorCore(string message) { }

}

/// <summary>Accelerometer from devicemotion accelerationIncludingGravity, reported in g.</summary>
public class BlazorAccelerometer : BlazorSensorBase, IAccelerometer
{
    public BlazorAccelerometer(IJSRuntime js) : base(js,"accelerometer") { }

	public event EventHandler<AccelerometerChangedEventArgs>? ReadingChanged;

	public event EventHandler? ShakeDetected
	{
		add { } // Shake detection is not implemented for the browser.
		remove { }
	}

	public async void Start(SensorSpeed sensorSpeed) => await StartCoreAsync(sensorSpeed);

	private protected override void OnReadingCore(string json)
	{
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;
		ReadingChanged?.Invoke(this, new AccelerometerChangedEventArgs(new AccelerometerData(
			root.GetProperty("x").GetDouble(),
			root.GetProperty("y").GetDouble(),
			root.GetProperty("z").GetDouble())));
	}
}

/// <summary>Gyroscope from devicemotion rotationRate, converted to rad/s.</summary>
public class BlazorGyroscope : BlazorSensorBase, IGyroscope
{
    public BlazorGyroscope(IJSRuntime js) : base(js, "gyroscope") { }

	public event EventHandler<GyroscopeChangedEventArgs>? ReadingChanged;

	public async void Start(SensorSpeed sensorSpeed) => await StartCoreAsync(sensorSpeed);

	private protected override void OnReadingCore(string json)
	{
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;
		ReadingChanged?.Invoke(this, new GyroscopeChangedEventArgs(new GyroscopeData(
			root.GetProperty("x").GetDouble(),
			root.GetProperty("y").GetDouble(),
			root.GetProperty("z").GetDouble())));
	}
}

/// <summary>Orientation quaternion derived from deviceorientation Euler angles.</summary>
public class BlazorOrientationSensor : BlazorSensorBase, IOrientationSensor
{
    public BlazorOrientationSensor(IJSRuntime js) : base(js, "orientation") { }

	public event EventHandler<OrientationSensorChangedEventArgs>? ReadingChanged;

	public async void Start(SensorSpeed sensorSpeed) => await StartCoreAsync(sensorSpeed);

	private protected override void OnReadingCore(string json)
	{
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;
		ReadingChanged?.Invoke(this, new OrientationSensorChangedEventArgs(new OrientationSensorData(
			root.GetProperty("x").GetDouble(),
			root.GetProperty("y").GetDouble(),
			root.GetProperty("z").GetDouble(),
			root.GetProperty("w").GetDouble())));
	}
}

/// <summary>Compass heading from deviceorientationabsolute (or webkitCompassHeading on iOS).</summary>
public class BlazorCompass : BlazorSensorBase, ICompass
{
    public BlazorCompass(IJSRuntime js) : base(js, "compass") { }

	public event EventHandler<CompassChangedEventArgs>? ReadingChanged;

	public async void Start(SensorSpeed sensorSpeed) => await StartCoreAsync(sensorSpeed);

	public async void Start(SensorSpeed sensorSpeed, bool applyLowPassFilter) => await StartCoreAsync(sensorSpeed);

	private protected override void OnReadingCore(string json)
	{
		using var doc = JsonDocument.Parse(json);
		ReadingChanged?.Invoke(this, new CompassChangedEventArgs(new CompassData(
			doc.RootElement.GetProperty("heading").GetDouble())));
	}
}
