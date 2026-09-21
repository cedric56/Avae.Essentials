using Microsoft.Extensions.DependencyInjection;

namespace Avae.Essentials;

public class CircuitServiceAccessor
{
    public static IServiceProvider? Provider { get; set; }

    public TService GetRequiredService<TService>() where TService : class
    {
        if (Provider == null) throw new InvalidOperationException("CircuitServiceAccessor.Provider is not been set.");
        return Provider.GetRequiredService<TService>();
    }
}