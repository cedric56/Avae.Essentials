using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

internal class BlazorMediaPicker : IAvaeMediaPicker
{
    public bool IsCaptureSupported => false;

    public Task<FileResult?> CaptureAsync(bool isPhoto, MediaPickerOptions? options = null)
    => throw new FeatureNotSupportedException("Camera capture is not supported in the browser backend.");

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

    public Task<FileResult?> CapturePhotoAsync(MediaPickerOptions? options = null) =>
        throw new FeatureNotSupportedException("Camera capture is not supported in the browser backend.");

    public Task<FileResult?> CaptureVideoAsync(MediaPickerOptions? options = null) =>
        throw new FeatureNotSupportedException("Camera capture is not supported in the browser backend.");

}
