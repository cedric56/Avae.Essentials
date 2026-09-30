using Avae.Essentials;
using Microsoft.JSInterop;
using Microsoft.Maui.Networking;

public sealed class BlazorConnectivity(IJSRuntime js) : IConnectivity, IAsyncDisposable
{
    public sealed record BrowserState(bool Online, string Type, bool SaveData);

    private volatile State _state = State.Unknown;
    private DotNetObjectReference<BlazorConnectivity>? _ref;

    public NetworkAccess NetworkAccess => _state.Access;
    public IEnumerable<ConnectionProfile> ConnectionProfiles => _state.Profiles;
    public event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChanged;

    // Idempotent: safe to call from several places.
    public async Task InitializeAsync()
    {
        var snapshot = await BlazorEssentialsInterop.InvokeWithRetryAsync<BrowserState?>(js, "connectivityGetSnapshot");
        if (snapshot is not null)
            Apply(snapshot, false);

        _ref = (await BlazorEssentialsInterop.SubscribeWithRetryAsync(js,
            async () => await BlazorEssentials.InvokeCoreAsync(CircuitServiceAccessor.Runtime),
            await BlazorEssentials.InitializeAsync(js),
            "connectivitySubscribe",
            this,
            _ref)).Reference;
    }

    [JSInvokable]
    public void OnBrowserChanged(BrowserState s) => Apply(s, raise: true);

    private void Apply(BrowserState s, bool raise)
    {
        var access = !s.Online ? NetworkAccess.None
                   : s.SaveData ? NetworkAccess.ConstrainedInternet
                   : NetworkAccess.Internet;

        var profile = s.Type switch
        {
            "wifi" => ConnectionProfile.WiFi,
            "cellular" => ConnectionProfile.Cellular,
            "ethernet" => ConnectionProfile.Ethernet,
            "bluetooth" => ConnectionProfile.Bluetooth,
            _ => ConnectionProfile.Unknown
        };

        var profiles = s.Online ? new[] { profile } : Array.Empty<ConnectionProfile>();
        _state = new State(access, profiles);

        if (raise)
            ConnectivityChanged?.Invoke(this, new ConnectivityChangedEventArgs(access, profiles));
    }

    private sealed record State(NetworkAccess Access, IReadOnlyList<ConnectionProfile> Profiles)
    {
        public static readonly State Unknown = new(NetworkAccess.Unknown, Array.Empty<ConnectionProfile>());
    }

    public async ValueTask DisposeAsync()
    {
        _ref?.Dispose();
        await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "connectivityUnsubscribe");
    }
}