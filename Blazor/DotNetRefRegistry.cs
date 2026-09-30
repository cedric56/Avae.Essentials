using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace Avae.Essentials;

internal static class DotNetRefRegistry<T> where T : class
{
    private static readonly ConditionalWeakTable<IJSRuntime, DotNetObjectReference<T>> _refs = new();

    /// <summary>
    /// Returns the DotNetObjectReference tracked for this specific runtime, creating one
    /// if none exists yet. Safe to call repeatedly; each IJSRuntime gets exactly one reference.
    /// </summary>
    public static DotNetObjectReference<T> GetOrCreate(IJSRuntime js, T instance)
        => _refs.GetValue(js, _ => DotNetObjectReference.Create(instance));

    /// <summary>
    /// Disposes and removes the reference for a specific runtime, e.g. when that circuit closes.
    /// </summary>
    public static void Remove(IJSRuntime js)
    {
        if (_refs.TryGetValue(js, out var reference))
        {
            reference.Dispose();
            _refs.Remove(js);
        }
    }
}
