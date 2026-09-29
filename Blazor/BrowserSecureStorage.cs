using Microsoft.JSInterop;
using Microsoft.Maui.Storage;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>
/// Secure storage backed by localStorage with AES-GCM encryption via WebCrypto.
/// The encryption key is non-extractable and lives in IndexedDB, so stored values
/// are unreadable outside this origin's browser context. This is best-effort:
/// any script running on the same origin can decrypt values, and clearing site
/// data destroys the key (existing values then read as null).
/// </summary>
public class BrowserSecureStorage(IJSRuntime js) : ISecureStorage
{
    const string KeyPrefix = "maui:securestorage:";

	public async Task<string?> GetAsync(string key)
	{
		return await BrowserEssentials.SecureGetAsync(js, KeyPrefix + key).ConfigureAwait(false);
	}

	public async Task SetAsync(string key, string value)
	{
		await BrowserEssentials.SecureSetAsync(js, KeyPrefix + key, value).ConfigureAwait(false);
	}

	public bool Remove(string key)
	{
		BrowserEssentials.EnsureInitialized();
		var storageKey = KeyPrefix + key;
		var exists = BrowserEssentials.PrefGet(js, storageKey) is not null;
        BrowserEssentials.PrefRemove(js, storageKey);
		return exists;
	}

	public async void RemoveAll()
	{
		BrowserEssentials.EnsureInitialized();
		foreach (var storageKey in BrowserEssentials.PrefKeys(js, KeyPrefix))
            BrowserEssentials.PrefRemove(js, storageKey);
	}
}
