using Avae.Essentials;
using Avalonia.Controls.Maui.Essentials;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

internal partial class AvaeEmail : IAvaeEmail
{
    public bool IsComposeSupported => true;

    public Task ComposeAsync(EmailMessage? message) =>
        PlatformComposeAsync(message);

    internal static async Task<string> GetUri(EmailMessage? message)
    {
        if (message == null)
        {
            return "mailto:";
        }
        else if (message.BodyFormat == EmailBodyFormat.PlainText &&
            (message.Attachments == null || message.Attachments.Count == 0))
        {
            return await message.ConvertToMailTo();
        }
        else
        {
            return await message.ConvertToEml();
        }
    }

    public Task ComposeAsync(IEnumerable<FileBase> files, EmailMessage message)
    {
        var attachments = new List<EmailAttachment>();
        foreach (var file in files ?? [])
        {
            if (file is AvaloniaFileResult result)
                attachments.Add(new AvaeEmailAttachment(result));
            else
                attachments.Add(new EmailAttachment(file.FullPath));
        }
        message.Attachments = attachments;
        return ComposeAsync(message);
    }
}
