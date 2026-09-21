using Avalonia.Controls.Maui.Essentials;
using Avalonia.Platform.Storage;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace Avae.Essentials;

partial class AvaeShareFile : ShareFile
{
    private IStorageFile _storageFile;
    public AvaeShareFile(AvaloniaFileResult result)
        : base(result.FullPath, result.ContentType)
    {
        _storageFile = result.StorageFile;
        FileName = result.FileName;
    }

    public new Task<Stream> OpenReadAsync()
    {
        return _storageFile.OpenReadAsync();
    }
}
