using Microsoft.JSInterop;
using Microsoft.Maui.Storage;
using System.Globalization;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>Preferences backed by window.localStorage.</summary>
public class BrowserPreferences(IJSRuntime js) : IPreferences
{
    const string KeyPrefix = "maui:prefs:";

	static string GetContainer(string? sharedName) => KeyPrefix + (sharedName ?? "_default") + ":";

	static string GetStorageKey(string key, string? sharedName) => GetContainer(sharedName) + key;

	public bool ContainsKey(string key, string? sharedName = null)
	{
		BrowserEssentials.EnsureInitialized();
		return BrowserEssentials.PrefGet(js, GetStorageKey(key, sharedName)) is not null;
	}

	public void Remove(string key, string? sharedName = null)
	{
		BrowserEssentials.EnsureInitialized();
        BrowserEssentials.PrefRemove(js, GetStorageKey(key, sharedName));
	}

	public void Clear(string? sharedName = null)
	{
		BrowserEssentials.EnsureInitialized();
		foreach (var storageKey in BrowserEssentials.PrefKeys(js, GetContainer(sharedName)))
            BrowserEssentials.PrefRemove(js, storageKey);
	}

	public void Set<T>(string key, T value, string? sharedName = null)
	{
		BrowserEssentials.EnsureInitialized();
		if (value is null)
		{
            BrowserEssentials.PrefRemove(js, GetStorageKey(key, sharedName));
			return;
		}

		var stored = value switch
		{
			DateTime dt => dt.ToString("O", CultureInfo.InvariantCulture),
			DateTimeOffset dto => dto.ToString("O", CultureInfo.InvariantCulture),
			IConvertible c => c.ToString(CultureInfo.InvariantCulture),
			_ => value.ToString() ?? string.Empty,
		};
        BrowserEssentials.PrefSet(js, GetStorageKey(key, sharedName), stored);
	}

	public T Get<T>(string key, T defaultValue, string? sharedName = null)
	{
		BrowserEssentials.EnsureInitialized();
		var stored = BrowserEssentials.PrefGet(js, GetStorageKey(key, sharedName));
		if (stored is null)
			return defaultValue;

		try
		{
			var type = typeof(T);
			var underlying = Nullable.GetUnderlyingType(type) ?? type;
			if (underlying == typeof(DateTime))
				return (T)(object)DateTime.Parse(stored, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
			if (underlying == typeof(DateTimeOffset))
				return (T)(object)DateTimeOffset.Parse(stored, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
			return (T)Convert.ChangeType(stored, underlying, CultureInfo.InvariantCulture);
		}
		catch
		{
			return defaultValue;
		}
	}
}
