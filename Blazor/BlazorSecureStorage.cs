using BlazorNative.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Avae.Essentials;

internal class BlazorSecureStorage(CircuitServiceAccessor circuitServiceAccessor) : ISecureStorage
{
    private static readonly TaskFactory _myTaskFactory = new
(CancellationToken.None,
               TaskCreationOptions.None,
               TaskContinuationOptions.None,
               TaskScheduler.Default);

    public static TResult RunSync<TResult>(Func<Task<TResult>> func)
    {
        return _myTaskFactory
          .StartNew(func)
          .Unwrap()
          .GetAwaiter()
          .GetResult();
    }

    public async Task<string?> GetAsync(string key)
    {
        var secureStorage = circuitServiceAccessor.GetRequiredService<BlazorNative.Device.ISecureStorage>();
        var secret = await secureStorage.GetAsync(key);
        return secret.Value;
    }

    public bool Remove(string key)
    {
        var secureStorage = circuitServiceAccessor.GetRequiredService<BlazorNative.Device.ISecureStorage>();
        return SecureStorageStatus.Ok == RunSync(async () => await secureStorage.DeleteAsync(key));
    }

    public void RemoveAll()
    {

    }

    public async Task SetAsync(string key, string value)
    {
        var secureStorage = circuitServiceAccessor.GetRequiredService<BlazorNative.Device.ISecureStorage>();
        await secureStorage.SetAsync(key, value);
    }
}
