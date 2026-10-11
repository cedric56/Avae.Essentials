> [!WARNING]
> **Avae.Essentials is preview software.** Pin a package version when integrating. Feature packs (`Avalonia`, `Blazor`) are opt-in via MSBuild targets.

# Avae.Essentials

Cross-host implementations of **.NET MAUI Essentials**-style APIs so the same ViewModels can run on **Avalonia**, **Blazor / WebAssembly**, and platform backends (Windows, Linux, macOS, mobile TFMs).

Contracts stay aligned with `Microsoft.Maui.Essentials` (`IGeolocation`, `IClipboard`, `IVibration`, …). Host code is registered through DI and `EssentialsAccessors` (`UnsafeAccessor` into MAUI static facades).

**License:** MIT — [LICENSE.txt](LICENSE.txt)

---

## Package layout

| Path | Role |
|------|------|
| `lib/<tfm>/Avae.Essentials.dll` | Core registration, accessors, shared helpers |
| `lib/<tfm>/Avalonia/**/*.avalonia.cs` | Avalonia / desktop host sources (opt-in) |
| `lib/<tfm>/Blazor/**` | Blazor implementations + JS module (opt-in) |
| `buildTransitive/Avae.Essentials.targets` | Conditionally compiles feature packs |

**TFMs:** `net11.0`, `net11.0-browser`, `net11.0-ios`, `net11.0-android`, `net11.0-macos`, `net11.0-windows10.0.19041`

### Enable Avalonia feature pack

```xml
<PropertyGroup>
  <AvaeFeatures>$(AvaeFeatures);AVALONIA;</AvaeFeatures>
</PropertyGroup>
```

Blazor sources are included automatically when the Web / Blazor WebAssembly SDK is used.

**Host apps** that call `UseAvaloniaEssentials()` must restore **Avalonia** and **Avalonia.Controls.Maui.Essentials** at runtime (the library may use `PrivateAssets=compile` so pure ViewModel projects are not forced to take Avalonia).

---

## Quick start

### Avalonia

```csharp
services.UseAvaloniaEssentials();
```

### Blazor

Register Blazor essentials (see `Blazor/` helpers), ensure `BlazorEssentials.js` is in `wwwroot`, and call `InitializeAsync()` on preferences before first read/write.

### MAUI-only

Prefer official **Microsoft.Maui.Essentials**. Use this package when sharing hosts with Avalonia/Blazor.

---

## Capability matrix (source-level, preview)

| Area | Windows (Avalonia pack) | Browser | Linux / macOS / mobile TFMs |
|------|-------------------------|---------|-----------------------------|
| Geolocation | WinRT `Geolocator` | JS geolocation | Backend / platform packages |
| Permissions | Partial Windows impl | Limited | Backend |
| Vibration | WinRT `VibrationDevice` when present; else `IsSupported == false` | JS `vibrate` | Backend |
| Clipboard / Share / Launcher | Implemented | Partial | Backend |
| Secure storage | DPAPI / app data | localStorage bridge | Backend |
| Sensors | Partial WinRT | JS DeviceMotion / orientation | Backend |
| File / Media picker | Avalonia helpers | Partial | Backend |

Desktop PCs usually have **no vibrator**: `IVibration.IsSupported` is `false` (does not throw).

---

## Architecture

1. **`EssentialsAccessors`** — `UnsafeAccessor` to MAUI `SetDefault` / `SetCurrent`. Pin `Microsoft.Maui.Essentials`; upgrades can break bindings.
2. **`SetDefaults`** — accepts **nullable** implementations; **null entries are skipped** (facade left unchanged) instead of forcing `null!` into the static default.
3. **Feature packs** — `*.avalonia.cs` / `Blazor/**` are not compiled into the main DLL by default; consumers opt in via targets.
4. **Vibration** — resolved via reflection to `Windows.Phone.Devices.Notification.VibrationDevice` so non-Windows TFMs compile without the Phone contract.

---

## Recent fixes (audit)

| Issue | Fix |
|-------|-----|
| Vibration `NotImplementedException("WINUI")` / `IsSupported` throw | Safe `TryGetDevice` + reflection; `IsSupported` never throws |
| `SetGeocolation` typo | Renamed **`SetGeolocation`** |
| `null!` registrations wiping defaults | Nullable `SetDefaults` + **set only if non-null** |
| `VersionTrackingDefault` all throws | Safe defaults from assembly version |
| `AppInfoDefault.Version` null | `new Version(0, 0)` |
| SecureStorage `Alias` early `AppInfo` | **Lazy** `Alias` property |
| Blazor Preferences `async void` persist | `Task` helpers + discard |
| Blazor Sensors `async void Start` | `void Start` → `_ = StartCoreAsync(...)` |
| Clipboard `SetText(null)` | Treat null as `string.Empty` |
| CsWinRT1033 NLM COM / NU5128 noise | `#pragma` / `NoWarn` where appropriate |

---

## Remaining limitations

- **AOT + classic COM** connectivity fallback remains non-ideal (CsWinRT1033 suppressed).
- **AndroidX / APT2260** on `net11.0-android` if you multi-target Android without a full MAUI-aligned AndroidX graph — prefer host app ownership of AndroidX, or drop Android TFM from this package if unused.
- **NU1608** Fragment vs Fragment.Ktx: align versions with MAUI in the **app** `Directory.Packages.props`.
- **Blazor** still has process-level statics / circuit lifecycle edge cases; prefer one essentials scope per circuit where possible.
- **Pack paths**: `TfmSpecificPackageFile` must evaluate with a **single** `$(TargetFramework)` (per-TFM pack target) to avoid *Illegal characters in path* (`;` in TFM list).
- Prefer conditioning package content on **`IsWindowsTFM`**, not only **`IsWindowsOS`** (build machine).

---

## Development

```bash
dotnet restore
dotnet build -f net11.0
dotnet pack -c Release
```

Test the TFM and host you ship (Avalonia on Windows, Blazor in a real browser, etc.).

---

## Relationship to MAUI Essentials

Same interface shapes; extra hosts for Avalonia and Blazor. Mobile TFMs often defer to official MAUI/platform packages (“backend”). Not a drop-in guarantee of full MAUI parity on every device.
