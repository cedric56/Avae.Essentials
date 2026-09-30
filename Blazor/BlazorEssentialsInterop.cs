using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

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
    /// Subscribes via JS, retrying once on a dead circuit. Returns the DotNetObjectReference
    /// that ended up successfully registered — the caller must store it and dispose it later.
    /// </summary>
    public static async Task<(IJSObjectReference Module, DotNetObjectReference<T> Reference)> SubscribeWithRetryAsync<T>(
        IJSRuntime js,
        Func<Task<IJSObjectReference>> importModule,
        IJSObjectReference module,
        string identifier,
        T instance,
        DotNetObjectReference<T>? previousRef,   // no `ref` — just read the old value in
        params object?[] extraArgs) where T : class
    {
        previousRef?.Dispose();
        var reference = DotNetObjectReference.Create(instance);
        var args = extraArgs.Prepend(reference).ToArray();

        try
        {
            await module.InvokeVoidAsync(identifier, args);
            return (module, reference);
        }
        catch (JSDisconnectedException)
        {
            reference.Dispose();
            var newModule = await importModule();
            reference = DotNetObjectReference.Create(instance);
            args = extraArgs.Prepend(reference).ToArray();
            await newModule.InvokeVoidAsync(identifier, args);
            return (newModule, reference);
        }
    }

    public static async Task<IJSObjectReference?> GetModuleWithRetryAsync(IJSRuntime js)
    {
        try
        {
            return await BlazorEssentials.InitializeAsync(js);
        }
        catch (JSDisconnectedException)
        {
            try
            {
                // Re-import the module on the (possibly reconnected) circuit
                // and retry exactly once.
                return await BlazorEssentials.InvokeCoreAsync(CircuitServiceAccessor.Runtime);
            }
            catch (JSDisconnectedException)
            {
                // Circuit is gone for good — nothing more we can do.
                // Swallow: callers of a fire-and-forget announce shouldn't crash.
            }
        }

        return null;
    }


    /// <summary>
    /// Invokes a void JS function with one automatic re-import + retry on
    /// <see cref="JSDisconnectedException"/>.
    /// </summary>
    public static async Task InvokeVoidWithRetryAsync(
        IJSRuntime js, string identifier, params object?[] args)
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

    public static async Task<TResult?> InvokeWithRetryAsync<
       [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.PublicFields |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
    TResult>(
       IJSRuntime js, string identifier, params object?[] args)
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