using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;
using Windows.Foundation.Metadata;

namespace Avae.Essentials;

partial class SmsImplementation : ISms
{
    public Task ComposeAsync() =>
        ComposeAsync(null);

    public Task ComposeAsync(SmsMessage? message)
    {
        if (!IsComposeSupported)
            throw new FeatureNotSupportedException();

        message ??= new SmsMessage();

        message.Recipients ??= new List<string>();

        return PlatformComposeAsync(message);
    }
}

[SupportedOSPlatform("windows10.0.10240")]
partial class SmsImplementation : ISms
{
    public bool IsComposeSupported
        => ApiInformation.IsTypePresent("Windows.ApplicationModel.Chat.ChatMessageManager");

    Task PlatformComposeAsync(SmsMessage message)
    {
        var chat = new ChatMessage();
        if (!string.IsNullOrWhiteSpace(message?.Body))
            chat.Body = message.Body;

        foreach (var recipient in message?.Recipients ?? [])
            chat.Recipients.Add(recipient);

        return ChatMessageManager.ShowComposeSmsMessageAsync(chat).AsTask();
    }
}
