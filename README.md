# Avae.Essentials

Cross-platform **device & platform APIs** for the [Avae](https://github.com/cedric56/Avae.Abstractions) stack — sensors, connectivity, clipboard, geolocation, secure storage, share, and more.

API surface is intentionally close to **.NET MAUI Essentials**, so shared ViewModels can call the same capabilities on:

| Target | TFM examples |
|--------|----------------|
| Desktop / shared | `net11.0` |
| Browser (WASM) | `net11.0-browser*` |
| Android | `net11.0-android*` |
| iOS / Mac Catalyst style | `net11.0-ios*` |
| macOS | `net11.0-macos*` |
| Windows | `net11.0-windows10.0.19041` |

> **Status:** preview (`1.0.0-preview.1`)

---

## Install

```xml
<PackageReference Include="Avae.Essentials" Version="1.0.0-preview.1" />
```

Platform packs are selected via multi-targeting. Prefer **ProjectReference** while developing against the Avae monorepo samples.

---

## Quick start

```csharp
using Avae.Essentials;
using Microsoft.Extensions.DependencyInjection;

services.UseEssentials();
```

This registers the default platform implementations (and related accessors) for the current TFM.

### Blazor / web host

When the consuming project is a **web SDK** project (`UsingMicrosoftNETSdkWeb`), `build/Avae.Essentials.targets` can:

- pull Blazor-oriented package references (sensors, clipboard, file system access, share, …);
- compile optional sources under `Blazor/`.

```csharp
// typical Blazor host
services.UseEssentials();
// or host-specific helpers if exposed in your version (e.g. UseBlazorEssentials)
```

---

## Capability areas

Folders map roughly one-to-one with features (non-exhaustive):

| Area | Examples |
|------|----------|
| **Sensors** | Accelerometer, Barometer, Compass, Gyroscope, Magnetometer, OrientationSensor |
| **Device** | Battery, DeviceDisplay, DeviceInfo, Flashlight, HapticFeedback, Vibration |
| **Network** | Connectivity |
| **Data & storage** | Preferences, SecureStorage, FileSystem, FilePicker |
| **Location** | Geolocation, Geocoding, Map |
| **UI / OS integration** | Clipboard, Share, Launcher, Browser, Email, PhoneDialer, Screenshot, MediaPicker, Contacts, AppActions, AppInfo |
| **A11y** | SemanticScreenReader |
| **Permissions** | Permissions helpers |
| **Platform** | Platform-specific wiring |

Shared helpers also include:

- **`EssentialsAccessors`** — static-style access patterns familiar from MAUI Essentials  
- **`AvaeDispatcher`** — marshal work to the UI / main context when required  
- **`ContentTypeResolver` / `CountryResolver`** — small utilities used by media and locale flows  
- **`CircuitServiceAccessor`** — Blazor circuit / DI bridging when needed  

Concrete interfaces for a subset of features live under `Interfaces/` (e.g. email, media picker, share, file results).

---

## Usage pattern

Prefer **constructor injection** of the abstractions your ViewModel needs (when registered), or the accessors registered by `UseEssentials`:

```csharp
public partial class EssentialsViewModel(/* injected essentials services */)
{
    public async Task ShareTextAsync()
    {
        // Same idea as MAUI Essentials Share / Clipboard / Geolocation, etc.
        // Exact type names follow the package’s public API for your TFM.
    }
}
```

Not every API is available on every platform (browser vs mobile vs desktop). Check `SupportedOSPlatform` attributes and runtime availability; some calls no-op or throw `FeatureNotSupportedException`-style errors depending on the implementation.

---

## Feature packaging notes

- **`build/Avae.Essentials.targets`** is shipped as **`buildTransitive`** (packed as `None`, not `Content`) to reduce Android **XA0101** issues.
- Blazor-specific `.cs` files are excluded from the main compile and optionally included for web projects via the targets file.
- Platform implementations use conditional compile (`*.android.cs`, `*.browser.cs`, `*.windows.cs`, `*.desktop.cs`, …) inside feature folders.

---

## Relationship to MAUI Essentials

| | MAUI Essentials | Avae.Essentials |
|--|-----------------|-----------------|
| API style | Static + interfaces | Closely aligned |
| Hosts | MAUI | Avalonia, MAUI, Blazor, desktop, browser |
| DI | Optional | `UseEssentials()` registration |
| Goal | First-party MAUI | Shared Avae ViewModels across UI stacks |

Use **MAUI Essentials** when you only ship MAUI. Use **Avae.Essentials** when the same code must run under Avalonia / Blazor / multi-host Avae samples.

---

## Limitations (preview)

- Surface area is large; maturity varies by platform and API.
- Browser support depends on Web APIs / third-party Blazor packages pulled by the targets file.
- Permissions and background behavior differ strongly between OS — always test on device.
- Version is **preview**; expect API adjustments before 1.0.

---

## License

MIT — see [LICENSE.txt](LICENSE.txt).

---

## Related

- [Avae.Abstractions](https://github.com/cedric56/Avae.Abstractions) — samples (`Example`, `Example.Maui`, `Example.BlazorAssembly`, …)
- [Avae.Services](https://github.com/cedric56/Avae.Services) — UI service contracts (dialogs, notifications)
- [Microsoft.Maui.Essentials](https://learn.microsoft.com/dotnet/maui/platform-integration/) — conceptual counterpart
