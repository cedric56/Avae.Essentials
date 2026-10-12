using Avae.Essentials;
using Avalonia.Controls.Maui.Essentials;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Avae.Essentials;

internal partial class AvaeEmail : IAvaeEmail
{
    private static async Task PlatformComposeAsync(EmailMessage? message)
    {
        var uri = await GetUri(message);
        bool isMailTo = uri.StartsWith("mailto:");
        if (!isMailTo)
        {
            var path = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.eml");
            await File.WriteAllTextAsync(path, uri);
            uri = path;
        }

        try
        {
            ProcessStartInfo? info = null;

            if (OperatingSystem.IsWindows())
            {
                info = new ProcessStartInfo(uri) { UseShellExecute = true };
            }
            else if (OperatingSystem.IsLinux())
            {
                info = new ProcessStartInfo("xdg-open", $"\"{uri}\"") { UseShellExecute = false };
            }
            else if (OperatingSystem.IsMacOS())
            {
                info = new ProcessStartInfo("open", $"\"{uri}\"") { UseShellExecute = false };
            }

            if (info == null)
                return;

            using var process = Process.Start(info);
            if (process != null) await process.WaitForExitAsync();
        }
        finally
        {
            if (!isMailTo)
            {
                var emlPath = uri;
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMinutes(5));
                    try { File.Delete(emlPath); } catch { }
                });
            }
        }
    }

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
