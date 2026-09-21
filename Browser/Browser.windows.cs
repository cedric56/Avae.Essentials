#nullable enable
using Microsoft.Maui.ApplicationModel;
using System.Runtime.Versioning;

namespace Avae.Essentials;

[SupportedOSPlatform("windows10.0.10240")]
partial class BrowserImplementation : IBrowser
{
    public Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options) =>
        global::Windows.System.Launcher.LaunchUriAsync(uri).AsTask();
}
