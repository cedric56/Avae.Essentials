using Avalonia.Controls.Maui.Essentials;
using Avalonia.Platform.Storage;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using System.IO;
using System.Threading.Tasks;

namespace Avae.Essentials;

public partial class AvaeShareFile : ShareFile
{
    private IStorageFile? _storageFile;

    public AvaeShareFile(AvaloniaFileResult result)
        : base(result.FullPath, result.ContentType)
    {
        _storageFile = result.StorageFile;
        FileName = result.FileName;
    }

    public new Task<Stream> OpenReadAsync()
    {
        if (_storageFile != null)
            return _storageFile.OpenReadAsync();
        return Task.FromResult<Stream>(File.OpenRead(FullPath));
    }

    public Task<Stream> OpenFileReadAsync()
    {
        if (_storageFile != null)
            return _storageFile.OpenReadAsync();
        return Task.FromResult<Stream>(File.OpenRead(FullPath));
    }
}
