using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.JSInterop;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

/// <summary>
/// Preferences backed by window.localStorage. All values are loaded into memory once via
/// <see cref="InitializeAsync"/>, so the synchronous <see cref="IPreferences"/> surface never
/// calls JS interop directly. Writes update the cache immediately and persist to localStorage
/// in the background (synchronously on WebAssembly, fire-and-forget on Blazor Server).
/// </summary>
public sealed class BlazorPreferences(IJSRuntime js) : IPreferences
{
    private const string KeyPrefix = "maui:prefs:";

    private readonly ConcurrentDictionary<string, string> _cache = new();
    
    private static string GetContainer(string? sharedName) => KeyPrefix + (sharedName ?? "_default") + ":";
    private static string GetStorageKey(string key, string? sharedName) => GetContainer(sharedName) + key;

    /// <summary>
    /// Loads all existing preference entries from localStorage into memory. Idempotent.
    /// Must complete before any Get/Set/Remove/Clear/ContainsKey call.
    /// </summary>
    public async Task InitializeAsync()
    {
        var all = await BlazorEssentialsInterop.InvokeWithRetryAsync<Dictionary<string, string>>(js, "prefsGetAll", KeyPrefix);
        foreach (var (k, v) in all ?? [])
            _cache[k] = v;
    }

    public bool ContainsKey(string key, string? sharedName = null)
        => _cache.ContainsKey(GetStorageKey(key, sharedName));

    public void Remove(string key, string? sharedName = null)
    {
        var storageKey = GetStorageKey(key, sharedName);
        if (_cache.TryRemove(storageKey, out _))
            PersistRemove(storageKey);
    }

    public void Clear(string? sharedName = null)
    {
        var prefix = GetContainer(sharedName);
        foreach (var storageKey in _cache.Keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList())
        {
            if (_cache.TryRemove(storageKey, out _))
                PersistRemove(storageKey);
        }
    }

    public void Set<T>(string key, T value, string? sharedName = null)
    {
        var storageKey = GetStorageKey(key, sharedName);

        if (value is null)
        {
            if (_cache.TryRemove(storageKey, out _))
                PersistRemove(storageKey);
            return;
        }

        var stored = value switch
        {
            DateTime dt => dt.ToString("O", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToString("O", CultureInfo.InvariantCulture),
            IConvertible c => c.ToString(CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty,
        };

        _cache[storageKey] = stored;
        PersistSet(storageKey, stored);
    }

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        if (!_cache.TryGetValue(GetStorageKey(key, sharedName), out var stored))
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

    // ── Persistence: sync on WASM, fire-and-forget on Server ──────────────

    private async void PersistSet(string storageKey, string value)
    {
        await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "prefsSet", storageKey, value);
    }

    private async void PersistRemove(string storageKey)
    {
        await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(js, "prefsRemove", storageKey);
    }
}
