using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace Avae.Essentials;

partial class AvaeEmail
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
}

//   partial class EmailImplementation : IEmail
//   {
//       public Task ComposeAsync(EmailMessage? message)
//       {
//           if (!IsComposeSupported)
//               throw new FeatureNotSupportedException();

//           return PlatformComposeAsync(message);
//       }

//       internal static string GetMailToUri(EmailMessage message) =>
//           "mailto:?" + string.Join("&", Parameters(message));

//       static IEnumerable<string> Parameters(EmailMessage message)
//       {
//           if (message.To?.Count > 0)
//               yield return "to=" + Recipients(message.To);

//           if (message.Cc?.Count > 0)
//               yield return "cc=" + Recipients(message.Cc);

//           if (message.Bcc?.Count > 0)
//               yield return "bcc=" + Recipients(message.Bcc);

//           if (!string.IsNullOrWhiteSpace(message.Subject))
//               yield return "subject=" + Uri.EscapeDataString(message.Subject);

//           if (!string.IsNullOrWhiteSpace(message.Body))
//               yield return "body=" + Uri.EscapeDataString(message.Body);
//       }

//       static string Recipients(IEnumerable<string> addresses) =>
//           string.Join(",", addresses.Select(Uri.EscapeDataString));
//   }

//   partial class EmailImplementation : IEmail
//{
//	public bool IsComposeSupported
//		=> true;

//	async Task PlatformComposeAsync(EmailMessage message)
//	{
//		if (message != null && message.BodyFormat != EmailBodyFormat.PlainText)
//			throw new FeatureNotSupportedException("Windows can only compose plain text email messages.");

//		var platformEmailMessage = new PlatformEmailMessage();

//		if (!string.IsNullOrEmpty(message?.Body))
//			platformEmailMessage.Body = message.Body;

//		if (!string.IsNullOrEmpty(message?.Subject))
//			platformEmailMessage.Subject = message.Subject;

//		Sync(message?.To, platformEmailMessage.To);
//		Sync(message?.Cc, platformEmailMessage.CC);
//		Sync(message?.Bcc, platformEmailMessage.Bcc);

//		if (message?.Attachments?.Count > 0)
//		{
//			foreach (var attachment in message.Attachments)
//			{
//				var path = NormalizePath(attachment.FullPath);

//				platformEmailMessage.Attachments.Add(path);
//			}
//		}

//		await EmailHelper.ShowComposeNewEmailAsync(platformEmailMessage);
//	}

//	static string NormalizePath(string path)
//		=> path.Replace('/', Path.DirectorySeparatorChar);

//	void Sync(List<string> recipients, IList<PlatformEmailRecipient> nativeRecipients)
//	{
//		if (recipients == null)
//			return;

//		foreach (var recipient in recipients)
//		{
//			if (string.IsNullOrWhiteSpace(recipient))
//				continue;

//			nativeRecipients.Add(new PlatformEmailRecipient(recipient));
//		}
//	}
//}
