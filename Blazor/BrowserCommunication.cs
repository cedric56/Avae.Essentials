using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using System.Runtime.Versioning;
using System.Text;

namespace Avae.Essentials;

/// <summary>Email compose via a mailto: link handled by the user's configured mail client.</summary>
public class BrowserEmail(IJSRuntime js) : IEmail
{
    public bool IsComposeSupported => true;

	public async Task ComposeAsync(EmailMessage? message)
	{
		if (message?.Attachments is { Count: > 0 })
			throw new FeatureNotSupportedException("mailto: links cannot carry attachments.");
		if (message?.BodyFormat is EmailBodyFormat.Html)
			throw new FeatureNotSupportedException("mailto: links only support plain text bodies.");

		var url = new StringBuilder("mailto:");
		if (message?.To is { Count: > 0 })
			url.Append(string.Join(",", message.To.Select(Uri.EscapeDataString)));

		var query = new List<string>();
		if (message?.Cc is { Count: > 0 })
			query.Add("cc=" + string.Join(",", message.Cc.Select(Uri.EscapeDataString)));
		if (message?.Bcc is { Count: > 0 })
			query.Add("bcc=" + string.Join(",", message.Bcc.Select(Uri.EscapeDataString)));
		if (!string.IsNullOrEmpty(message?.Subject))
			query.Add("subject=" + Uri.EscapeDataString(message.Subject));
		if (!string.IsNullOrEmpty(message?.Body))
			query.Add("body=" + Uri.EscapeDataString(message.Body));
		if (query.Count > 0)
			url.Append('?').Append(string.Join("&", query));

		await BrowserEssentials.NavigateToAsync(js, url.ToString());
	}
}

/// <summary>Phone dialing via a tel: link (effective on mobile browsers or with a desktop handler).</summary>
public class BrowserPhoneDialer(IJSRuntime js) : IPhoneDialer
{
    public bool IsSupported => true;

	public async void Open(string number)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(number);
		BrowserEssentials.EnsureInitialized();
		await BrowserEssentials.NavigateToAsync(js, "tel:" + Uri.EscapeDataString(number));
	}
}

/// <summary>SMS compose via an sms: link (effective on mobile browsers).</summary>
public class BrowserSms(IJSRuntime js) : ISms
{
    public bool IsComposeSupported => true;

	public async Task ComposeAsync(SmsMessage? message)
	{
		var url = new StringBuilder("sms:");
		if (message?.Recipients is { Count: > 0 })
			url.Append(string.Join(",", message.Recipients.Select(Uri.EscapeDataString)));
		if (!string.IsNullOrEmpty(message?.Body))
			url.Append("?body=").Append(Uri.EscapeDataString(message.Body));
		await BrowserEssentials.NavigateToAsync( js,url.ToString());
	}
}
