using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Devices.Sensors;
using System.Runtime.Versioning;
using System.Text.Json;

namespace Avae.Essentials;

/// <summary>
/// Shared plumbing for sensors backed by devicemotion/deviceorientation events.
/// SensorSpeed is ignored — browsers deliver events at their own fixed rate.
/// On iOS Safari the first Start triggers the motion-permission prompt and must be
/// called from a user gesture.
/// </summary>
public abstract class BrowserSensorBase(IJSRuntime js, string kind)
{
	readonly string kind = kind;

	public bool IsSupported
	{
		get
		{
			BrowserEssentials.EnsureInitialized();
			return BrowserEssentials.SensorIsSupported(js, kind);
		}
	}

	public bool IsMonitoring { get; private set; }

	private protected void StartCore(SensorSpeed sensorSpeed)
	{
		BrowserEssentials.EnsureInitialized();
		if (!IsSupported)
			throw new FeatureNotSupportedException($"The {kind} sensor is not available in this browser.");
		if (IsMonitoring)
			throw new InvalidOperationException($"The {kind} sensor is already being monitored.");
		IsMonitoring = true;
		_ = StartListenerAsync();
	}

	Task StartListenerAsync()
	{
		//var started = await BrowserEssentialsInterop.SensorStartAsync(js, kind, OnReading).ConfigureAwait(false);
		//if (!started)
		//	IsMonitoring = false;
		return Task.CompletedTask;
	}

	public async void Stop()
	{
		if (!IsMonitoring)
			return;
		BrowserEssentials.EnsureInitialized();
	await BrowserEssentials.SensorStopAsync(js, kind);
		IsMonitoring = false;
	}

	private protected abstract void OnReading(string json);
}

/// <summary>Accelerometer from devicemotion accelerationIncludingGravity, reported in g.</summary>
public class BrowserAccelerometer : BrowserSensorBase, IAccelerometer
{
    public BrowserAccelerometer(IJSRuntime js) : base(js,"accelerometer") { }

	public event EventHandler<AccelerometerChangedEventArgs>? ReadingChanged;

	public event EventHandler? ShakeDetected
	{
		add { } // Shake detection is not implemented for the browser.
		remove { }
	}

	public void Start(SensorSpeed sensorSpeed) => StartCore(sensorSpeed);

	private protected override void OnReading(string json)
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
public class BrowserGyroscope : BrowserSensorBase, IGyroscope
{
    public BrowserGyroscope(IJSRuntime js) : base(js, "gyroscope") { }

	public event EventHandler<GyroscopeChangedEventArgs>? ReadingChanged;

	public void Start(SensorSpeed sensorSpeed) => StartCore(sensorSpeed);

	private protected override void OnReading(string json)
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
public class BrowserOrientationSensor : BrowserSensorBase, IOrientationSensor
{
    public BrowserOrientationSensor(IJSRuntime js) : base(js, "orientation") { }

	public event EventHandler<OrientationSensorChangedEventArgs>? ReadingChanged;

	public void Start(SensorSpeed sensorSpeed) => StartCore(sensorSpeed);

	private protected override void OnReading(string json)
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
public class BrowserCompass : BrowserSensorBase, ICompass
{
    public BrowserCompass(IJSRuntime js) : base(js, "compass") { }

	public event EventHandler<CompassChangedEventArgs>? ReadingChanged;

	public void Start(SensorSpeed sensorSpeed) => StartCore(sensorSpeed);

	public void Start(SensorSpeed sensorSpeed, bool applyLowPassFilter) => StartCore(sensorSpeed);

	private protected override void OnReading(string json)
	{
		using var doc = JsonDocument.Parse(json);
		ReadingChanged?.Invoke(this, new CompassChangedEventArgs(new CompassData(
			doc.RootElement.GetProperty("heading").GetDouble())));
	}
}
