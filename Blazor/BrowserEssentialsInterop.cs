using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Avae.Essentials;


public sealed class ConnectivityCallback
{
    private readonly Action<bool> _onChanged;

    public ConnectivityCallback(Action<bool> onChanged) => _onChanged = onChanged;

    [JSInvokable]
    public void OnConnectivityChanged(bool isOnline) => _onChanged(isOnline);
}

public sealed class DisplayCallback
{
    private readonly Action _onChanged;

    public DisplayCallback(Action onChanged) => _onChanged = onChanged;

    [JSInvokable]
    public void OnDisplayChanged() => _onChanged();
}

public sealed class GeoCallback
{
    private readonly Action<string> _onSuccess;
    private readonly Action<string> _onError;

    public GeoCallback(Action<string> onSuccess, Action<string> onError)
    {
        _onSuccess = onSuccess;
        _onError = onError;
    }

    [JSInvokable] public void OnPosition(string json) => _onSuccess(json);
    [JSInvokable] public void OnError(string error) => _onError(error);
}

public sealed class BatteryCallback
{
    private readonly Action<string> _onChanged;

    public BatteryCallback(Action<string> onChanged) => _onChanged = onChanged;

    [JSInvokable]
    public void OnBatteryChanged(string json) => _onChanged(json);
}

public sealed class SensorCallback
{
    private readonly Action<string> _onChanged;

    public SensorCallback(Action<string> onChanged) => _onChanged = onChanged;

    [JSInvokable]
    public void OnSensorChanged(string json) => _onChanged(json);
}