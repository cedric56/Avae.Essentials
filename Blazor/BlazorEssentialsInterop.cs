using Microsoft.JSInterop;

namespace Avae.Essentials;

/// <summary>
/// Helpers for making JS interop calls resilient against circuit disconnection.
/// On <see cref="JSDisconnectedException"/>, the JS module is re-imported once
/// and the call is retried on the (possibly new) circuit. If it fails again,
/// the exception is swallowed so callers don't need to guard every call site.
/// </summary>
public static class BlazorEssentialsInterop
{
    /// <summary>
    /// Invokes a void JS function with one automatic re-import + retry on
    /// <see cref="JSDisconnectedException"/>.
    /// </summary>
    public static async Task InvokeVoidWithRetryAsync(
        IJSRuntime? js,
        string identifier,
        params object?[] args)
    {
        try
        {
            await (await BlazorEssentials.InitializeAsync(js)).InvokeVoidAsync(identifier, args).ConfigureAwait(false);
        }
        catch (JSDisconnectedException)
        {
            try
            {
                // Re-import the module on the (possibly reconnected) circuit
                // and retry exactly once.
                await (await BlazorEssentials.InvokeCoreAsync(CircuitServiceAccessor.Runtime)).InvokeVoidAsync(identifier, args).ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
                // Circuit is gone for good — nothing more we can do.
                // Swallow: callers of a fire-and-forget announce shouldn't crash.
            }
        }
    }

    public static async Task<TResult?> InvokeWithRetryAsync<TResult>(
        IJSRuntime? js,
        string identifier,
        params object?[] args)
    {
        try
        {
            return await (await BlazorEssentials.InitializeAsync(js)).InvokeAsync<TResult>(identifier, args).ConfigureAwait(false);
        }
        catch (JSDisconnectedException)
        {
            try
            {
                return await (await BlazorEssentials.InvokeCoreAsync(CircuitServiceAccessor.Runtime)).InvokeAsync<TResult>(identifier, args).ConfigureAwait(false);
            }
            catch (JSDisconnectedException)
            {
                return default;
            }
        }
    }
}