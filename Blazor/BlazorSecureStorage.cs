using Microsoft.JSInterop;
using Microsoft.Maui.Storage;
using System.Collections.Concurrent;

namespace Avae.Essentials;

/// <summary>
/// Secure storage backed by localStorage with AES-GCM encryption via WebCrypto.
/// The encryption key is non-extractable and lives in IndexedDB, so stored values
/// are unreadable outside this origin's browser context. This is best-effort:
/// any script running on the same origin can decrypt values, and clearing site
/// data destroys the key (existing values then read as null).
/// </summary>
public class BlazorSecureStorage(IJSRuntime js) : ISecureStorage
{
    const string KeyPrefix = "maui:securestorage:";

    private readonly ConcurrentDictionary<string, byte> _knownKeys = new();

    /// <summary>Loads the set of existing keys (not values). Idempotent.</summary>
    public async Task InitializeAsync()
    {
        var keys = await BlazorEssentialsInterop.InvokeWithRetryAsync<string[]>(js, "secureKeys", KeyPrefix);
        foreach (var k in keys ?? [])
            _knownKeys[k] = 0;
    }

    public async Task<string?> GetAsync(string key)
	{
		return await BlazorEssentialsInterop.InvokeWithRetryAsync<string?>(js, "secureGet", KeyPrefix + key).ConfigureAwait(false);
	}

	public async Task SetAsync(string key, string value)
	{
		await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "secureSet", KeyPrefix + key, value).ConfigureAwait(false);
	}

    public bool Remove(string key)
    {
        var storageKey = KeyPrefix + key;
        if (!_knownKeys.TryRemove(storageKey, out _))
            return false;

        PersistRemove(storageKey);
        return true;
    }

    public void RemoveAll()
    {
        foreach (var storageKey in _knownKeys.Keys.ToList())
        {
            if (_knownKeys.TryRemove(storageKey, out _))
                PersistRemove(storageKey);
        }
    }

    private async void PersistRemove(string storageKey)
    {
        await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "secureRemove", storageKey);
    }
}
