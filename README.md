# Avae.Essentials

Cross-platform device, browser, and platform APIs for Avalonia, .NET MAUI, Blazor/WebAssembly, Windows, Android, iOS, macOS, and Linux hosts.

The API surface intentionally follows **.NET MAUI Essentials** so shared Avae ViewModels can use the same abstractions across UI stacks.

> **Status:** preview — version `1.0.0-preview.3`
>
> **Important:** platform availability below describes the implementation/packaging present in this repository. Browser APIs additionally depend on the browser's Web API support, permissions, secure-context rules, and user gestures. Native backend capabilities should still be verified on the target OS/device.

## Targets

| Platform / host | Target | Implementation source |
|---|---|---|
| Android | `net11.0-android` | MAUI Essentials backend |
| iOS | `net11.0-ios` | MAUI Essentials backend |
| macOS | `net11.0-macos` | MAUI macOS Essentials backend |
| Windows | `net11.0-windows10.0.19041` | Avae Windows implementations |
| Browser / Blazor WebAssembly | `net11.0-browser` | Avae Blazor + browser Web APIs |
| Linux / Avalonia desktop | `net11.0` on Linux | Linux GTK4 Essentials backend |
| Generic desktop | `net11.0` | Host-OS dependent |

## Install

```xml
<PackageReference Include="Avae.Essentials" Version="1.0.0-preview.3" />
```

For development inside the Avae monorepo, a `ProjectReference` is recommended.

Register the implementations with dependency injection:

```csharp
services.UseEssentials();
```

For a Blazor/Web host:

```csharp
services.UseBlazorEssentials();
await BlazorEssentials.InitializeAsync(serviceProvider);
```

The package's `buildTransitive` targets copy the Blazor JavaScript module into the consuming web application's `wwwroot` when appropriate.

---

# Platform capability matrix

### Legend

- **Yes** — implementation is present for the target.
- **Browser** — explicit Blazor implementation exists; actual availability depends on the browser.
- **Backend** — delegated to the referenced platform Essentials package; OS/device support can vary.
- **Limited** — implementation exists but the browser/platform cannot expose the full MAUI capability.
- **No** — no implementation is exposed by the current target.

| Functionality | Android | iOS | macOS | Windows | Browser / Blazor | Linux / Avalonia |
|---|---|---|---|---|---|---|
| Accelerometer | Backend | Backend | Backend | Yes | Browser | Backend |
| App Actions | Backend | Backend | Backend | Yes | Browser | Backend |
| App Info | Backend | Backend | Backend | Yes | Browser | Backend |
| Barometer | Backend | Backend | Backend | Yes | Browser | Backend |
| Battery | Backend | Backend | Backend | Yes | Browser | Backend |
| Browser | Backend | Backend | Backend | Yes | Browser | Backend |
| Clipboard | Backend | Backend | Backend | Yes | Browser | Backend |
| Compass | Backend | Backend | Backend | Yes | Browser | Backend |
| Connectivity | Backend | Backend | Backend | Yes | Browser | Backend |
| Contacts | Backend | Backend | Backend | Yes | Browser | Limited — Contact Picker support varies |
| Device Display | Backend | Backend | Backend | Yes | Browser | Backend |
| Device Info | Backend | Backend | Backend | Yes | Browser | Backend |
| Email | Backend | Backend | Backend | Yes | Browser | Backend |
| File Picker | Backend | Backend | Backend | Yes | Browser | Backend |
| File System | Backend | Backend | Backend | Yes | Browser | Backend |
| Flashlight | Backend | Backend | Backend | Yes | Browser | Limited by browser APIs |
| Geocoding | Backend | Backend | Backend | Yes | Browser | Backend |
| Geolocation | Backend | Backend | Backend | Yes | Browser | Backend |
| Gyroscope | Backend | Backend | Backend | Yes | Browser | Backend |
| Haptic Feedback | Backend | Backend | Backend | Yes | Browser | Limited |
| Launcher | Backend | Backend | Backend | Yes | Browser | Backend |
| Magnetometer | Backend | Backend | Backend | Yes | Browser | Backend |
| Map | Backend | Backend | Backend | Yes | Limited — maps app launch is unsupported | Backend |
| Media Picker | Backend | Backend | Backend | Yes | Browser | Backend |
| Orientation Sensor | Backend | Backend | Backend | Yes | Browser | Backend |
| Permissions | Backend | Backend | Backend | Yes | No dedicated Blazor implementation | Backend |
| Phone Dialer | Backend | Backend | Backend | Yes | Browser | Backend |
| Preferences | Backend | Backend | Backend | Yes | Browser | Backend |
| Screenshot | Backend | Backend | Backend | Yes | Browser | Backend |
| Secure Storage | Backend | Backend | Backend | Yes | Browser | Limited — WebCrypto + IndexedDB/localStorage |
| Semantic Screen Reader | Backend | Backend | Backend | Yes | Browser | Backend |
| Share | Backend | Backend | Backend | Yes | Browser | Backend |
| SMS | Backend | Backend | Backend | Yes | Browser | Browser-dependent protocol support |
| Text to Speech | Backend | Backend | Backend | Yes | Browser | Browser/native backend dependent |
| Version Tracking | Backend | Backend | Backend | Yes | Browser | Backend |
| Vibration | Backend | Backend | Backend | Yes | Browser | Browser-dependent |
| WSL helpers | No | No | No | Windows/WSL host | No | Linux/WSL host |

### Browser implementation details

The Blazor implementation currently contains explicit browser-backed implementations for:

- App Actions
- App Info
- Barometer
- Battery
- Browser
- Clipboard
- Connectivity
- Contacts
- Device Display
- Device Info
- Email
- File Picker
- File System
- Flashlight
- Geocoding
- Geolocation
- Haptic Feedback
- Launcher
- Magnetometer
- Media Picker
- Phone Dialer
- Preferences
- Screenshot
- Secure Storage
- Semantic Screen Reader
- Share
- SMS
- Text to Speech
- Version Tracking
- Vibration
- Accelerometer
- Compass
- Gyroscope
- Orientation Sensor

Some browser capabilities are necessarily reduced compared with a native device. For example, map-app launching is explicitly unsupported, secure storage is best-effort WebCrypto storage, and sensor/location/camera APIs are subject to browser permissions and secure-context requirements.

## API areas

The source tree is organized by functionality:

- **Sensors:** Accelerometer, Barometer, Compass, Gyroscope, Magnetometer, Orientation Sensor
- **Device:** Battery, Device Display, Device Info, Flashlight, Haptic Feedback, Vibration
- **Network:** Connectivity
- **Storage:** Preferences, Secure Storage, File System, File Picker
- **Location:** Geolocation, Geocoding, Map
- **Communication:** Email, Phone Dialer, SMS, Contacts
- **Media:** Media Picker, Screenshot
- **Sharing / launching:** Share, Browser, Launcher
- **Application:** App Actions, App Info, Version Tracking
- **Accessibility:** Semantic Screen Reader
- **Permissions:** Permissions
- **Platform:** Platform and window helpers
- **Browser:** Blazor implementations and JavaScript interop

Shared helpers include `EssentialsAccessors`, `AvaeDispatcher`, `ContentTypeResolver`, `CountryResolver`, and Blazor circuit/DI helpers.

## Browser requirements

Browser-backed features can require:

- HTTPS / a secure context.
- A user gesture for APIs such as Web Share, camera access, and iOS motion/orientation permission.
- Browser permission grants for location, camera, contacts, or sensors.
- A browser that implements the corresponding Web API.

Feature detection is used where practical, but browser support is not uniform across Chromium, Firefox, Safari, and embedded WebViews.

## Known audit findings

The current source audit has identified these concrete follow-ups:

- **#3** — Blazor Essentials module/services mix process-global state with circuit-specific JavaScript state.
- **#4** — Blazor Preferences still uses fire-and-forget `async void` persistence.
- **#9** — Blazor geolocation watch callbacks use method names that do not exist on the C# implementation.
- **#10** — Blazor sensor callbacks use a JavaScript/.NET signature that does not match.
- **#11** — Blazor video capture currently calls the photo capture workflow.
- **#12** — Blazor MediaPicker stores captured data in a static field.
- **#13** — Blazor Text-to-Speech cancellation uses an effectively `async void` callback.
- **#14** — Blazor Device Display has a first-use race between subscription and wake-lock setup.

Earlier audit issues also cover the Blazor lifecycle, subscription cleanup, and global DI/static facade behavior.

These are tracked as separate GitHub issues so fixes can be reviewed independently.

## Development

```bash
dotnet build
dotnet test
```

The repository is multi-targeted; when validating a platform-specific feature, test the corresponding TFM and, for browser APIs, test the actual browser/device combination.

## Relationship to .NET MAUI Essentials

Avae.Essentials intentionally follows MAUI Essentials concepts and interfaces while providing additional host implementations for Avae/Avalonia/Blazor scenarios.

If an application only targets MAUI, the official MAUI Essentials implementation remains the simpler choice. Avae.Essentials is intended for applications sharing ViewModels and service abstractions across multiple UI stacks.

## Preview limitations

- This package is **preview** software.
- Platform support varies by API.
- Browser APIs are constrained by Web Platform security and permissions.
- Native backend support can vary by OS version and hardware.
- The capability matrix describes source-level availability; it is not a guarantee that every browser, device, or OS configuration supports every operation.

## License

MIT — see [LICENSE.txt](LICENSE.txt).

## Related projects

- [Avae.Abstractions](https://github.com/cedric56/Avae.Abstractions)
- [Avae.Services](https://github.com/cedric56/Avae.Services)
- [.NET MAUI Essentials](https://learn.microsoft.com/dotnet/maui/platform-integration/)
