using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

public sealed class VideoCaptureCoordinator
{
    public event Func<TaskCompletionSource<FileResult?>, Task>? RequestCapture;

    public Task<FileResult?> RequestVideoCaptureAsync()
    {
        var tcs = new TaskCompletionSource<FileResult?>(TaskCreationOptions.RunContinuationsAsynchronously);
        RequestCapture?.Invoke(tcs);
        return tcs.Task;
    }
}

internal class BlazorMediaPicker(CircuitServiceAccessor circuitServiceAccessor) : IMediaPicker
{
    public bool IsCaptureSupported => true;

    public async Task<FileResult?> CapturePhotoAsync(MediaPickerOptions? options = null)
    {
        var camera = circuitServiceAccessor.GetRequiredService<BlazorNative.Device.ICamera>();
        var result = await camera.CapturePhotoAsync();
        if (result.Status == BlazorNative.Core.CameraStatus.Captured
            && !string.IsNullOrWhiteSpace(result.Path))
            return new FileResult(result.Path);
        return null;
    }

    public Task<FileResult?> CaptureVideoAsync(MediaPickerOptions? options = null)
    {
        var coordinator = circuitServiceAccessor.GetRequiredService<VideoCaptureCoordinator>();
        return coordinator.RequestVideoCaptureAsync();
    }

    public Task<FileResult?> PickPhotoAsync(MediaPickerOptions? options = null)
    {
        var picker = circuitServiceAccessor.GetRequiredService<IFilePicker>();
        return picker.PickAsync(CreatePhotoPickerOptions(options));
    }

    public async Task<List<FileResult>> PickPhotosAsync(MediaPickerOptions? options = null)
    {
        var picker = circuitServiceAccessor.GetRequiredService<IFilePicker>();
        return [.. await picker.PickMultipleAsync(CreatePhotoPickerOptions(options)) ?? []];
    }

    public Task<FileResult?> PickVideoAsync(MediaPickerOptions? options = null)
    {
        var picker = circuitServiceAccessor.GetRequiredService<IFilePicker>();
        return picker.PickAsync(CreateVideoPickerOptions(options));
    }

    public async Task<List<FileResult>> PickVideosAsync(MediaPickerOptions? options = null)
    {
        var picker = circuitServiceAccessor.GetRequiredService<IFilePicker>();
        return [.. await picker.PickMultipleAsync(CreateVideoPickerOptions(options)) ?? []];
    }

    static PickOptions CreatePhotoPickerOptions(MediaPickerOptions? options, bool allowMultiple = false)
    {
        var files = new Dictionary<DevicePlatform, IEnumerable<string>>()
        {
            {
                DevicePlatform.Unknown, new List<string>()
                {
                    "*.jpg", "*.jpeg", "*.png", "*.gif", "*.bmp", "*.webp"
                }
            }
        };

        return new PickOptions
        {
            PickerTitle = options?.Title,
            FileTypes = new FilePickerFileType(files)
        };
    }

    static PickOptions CreateVideoPickerOptions(MediaPickerOptions? options, bool allowMultiple = false)
    {
        var files = new Dictionary<DevicePlatform, IEnumerable<string>>()
        {
            {
                DevicePlatform.Unknown, new List<string>()
                {
                    "*.mp4", "*.mov", "*.avi", "*.wmv", "*.mkv", "*.webm"
                }
            }
        };

        return new PickOptions
        {
            PickerTitle = options?.Title,
            FileTypes = new FilePickerFileType(files)
        };
    }
}
