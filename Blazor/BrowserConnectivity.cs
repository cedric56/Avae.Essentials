using Microsoft.JSInterop;
using Microsoft.Maui.Networking;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>
/// Connectivity backed by navigator.onLine plus online/offline events and, where
/// available, the Network Information API for the connection profile.
/// </summary>
public class BrowserConnectivity(IJSRuntime js) : IConnectivity
{
    bool watching;

	public NetworkAccess NetworkAccess
	{
		get
		{
			BrowserEssentials.EnsureInitialized();
			return BrowserEssentials.IsOnline(js) ? NetworkAccess.Internet : NetworkAccess.Unknown;
		}
	}

	public IEnumerable<ConnectionProfile> ConnectionProfiles
	{
		get
		{
			BrowserEssentials.EnsureInitialized();
			if (!BrowserEssentials.IsOnline(js))
				return [];

			// navigator.connection.type is only implemented on some platforms;
			// effectiveType ("4g" etc.) intentionally maps to Unknown.
			return BrowserEssentials.GetConnectionType(js) switch
			{
				"wifi" => [ConnectionProfile.WiFi],
				"cellular" => [ConnectionProfile.Cellular],
				"ethernet" => [ConnectionProfile.Ethernet],
				"bluetooth" => [ConnectionProfile.Bluetooth],
				_ => [ConnectionProfile.Unknown],
			};
		}
	}

	public event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChanged
	{
		add
		{
			EnsureWatching();
			connectivityChanged += value;
		}
		remove => connectivityChanged -= value;
	}

	event EventHandler<ConnectivityChangedEventArgs>? connectivityChanged;

	void EnsureWatching()
	{
		BrowserEssentials.EnsureInitialized();
		if (watching)
			return;
		watching = true;
		//BrowserEssentialsInterop.WatchConnectivityAsync(js, new DotNetObjectReference<ConnectivityCallback>(_ =>
		//	connectivityChanged?.Invoke(this, new ConnectivityChangedEventArgs(NetworkAccess, ConnectionProfiles))));
	}
}
