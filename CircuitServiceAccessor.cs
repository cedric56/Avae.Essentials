using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Avae.Essentials;

public class CircuitServiceAccessor
{
    public static IServiceProvider? Provider { get; set; }

    public static IJSRuntime? Runtime { get; set; }

    public TService GetRequiredService<TService>() where TService : class
    {
        if (Provider == null) throw new InvalidOperationException("CircuitServiceAccessor.Provider is not been set.");
        return Provider.GetRequiredService<TService>();
    }
}