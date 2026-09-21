using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

public interface IAvaeMediaPicker : IMediaPicker
{
    Task<FileResult?> CaptureAsync(bool isPhoto, MediaPickerOptions? options = null);
}
