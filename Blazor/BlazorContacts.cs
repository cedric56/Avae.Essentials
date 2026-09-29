using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Avae.Essentials;

/// <summary>Contacts are not accessible from the browser (the Contact Picker API is not broadly available).</summary>
public class BlazorContacts : IContacts
{
    public Task<Contact?> PickContactAsync() =>
        throw new FeatureNotSupportedException("Contacts are not available in the browser.");

    public Task<IEnumerable<Contact>> GetAllAsync(CancellationToken cancellationToken = default) =>
        throw new FeatureNotSupportedException("Contacts are not available in the browser.");
}
