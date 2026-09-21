using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

public interface IAvaeEmail : IEmail
{
    Task ComposeAsync(IEnumerable<FileBase> files, EmailMessage message);
}
