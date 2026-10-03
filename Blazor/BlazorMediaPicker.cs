using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static BlazorConnectivity;

namespace Avae.Essentials;

internal class BlazorMediaPicker : IAvaeMediaPicker
{
    DotNetObjectReference<BlazorMediaPicker>? _ref;
    public async Task InitializeAsync()
    {
        _ref?.Dispose();
        _ref = DotNetObjectReference.Create(this);
    }

    public bool IsCaptureSupported => true;

    public Task<FileResult?> CaptureAsync(bool isPhoto, MediaPickerOptions? options = null)
    => isPhoto ? CapturePhotoAsync(options) : CaptureVideoAsync(options);

    public async Task<FileResult?> PickPhotoAsync(MediaPickerOptions? options = null)
        => (await BlazorFilePicker.PickCoreAsync(
            multiple: false)).FirstOrDefault();

    public async Task<List<FileResult>> PickPhotosAsync(MediaPickerOptions? options = null)
    => (await BlazorFilePicker.PickCoreAsync(
        multiple: true) ?? []).ToList();

    public async Task<FileResult?> PickVideoAsync(MediaPickerOptions? options = null)
    => (await BlazorFilePicker.PickCoreAsync(
        multiple: false)).FirstOrDefault();

    public async Task<List<FileResult>> PickVideosAsync(MediaPickerOptions? options = null)
    => (await BlazorFilePicker.PickCoreAsync(
        multiple: true) ?? []).ToList();

    public async Task<FileResult?> CapturePhotoAsync(MediaPickerOptions? options = null)
    {
        var result = await BlazorEssentials.Module.InvokeAsync<string>("capturePhotoInPopup");
        if (!string.IsNullOrWhiteSpace(result))
        {
            await BlazorEssentials.Module.InvokeVoidAsync("sendBlobToDotNet", result, _ref);
            return new BlazorFileResult(result, ContentTypeResolver.Resolve(Path.GetFileName(result)), _data!);
        }
        return null;
    }

    public async Task<FileResult?> CaptureVideoAsync(MediaPickerOptions? options = null)
    {
        var result = await BlazorEssentials.Module.InvokeAsync<string>("capturePhotoInPopup");
        if (!string.IsNullOrWhiteSpace(result))
        {
            await BlazorEssentials.Module.InvokeVoidAsync("sendBlobToDotNet", result, _ref);
            return new BlazorFileResult(result, ContentTypeResolver.Resolve(Path.GetFileName(result)), _data!);
        }
        return null;
    }

    private static byte[]? _data;

    [JSInvokable]
    public void ReceiveBlobData(byte[] bytes)
    {
        _data = bytes;
    }
}
