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

    public byte[]? Data { get; private set; }

    public AvaeShareFile(AvaloniaFileResult result)
        : base(result.FullPath, result.ContentType)
    {
        _storageFile = result.StorageFile;
        FileName = result.FileName;
    }

    public AvaeShareFile(BlazorFileResult result)
        : base(result.FullPath, result.ContentType)
    {
        FileName = result.FileName;
        Data = result.Data;
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

public class BlazorFileResult : FileResult
{
    public byte[] Data { get; private set; }

    public BlazorFileResult(string fullPath, string contentType, byte[] data)
        : base(fullPath, contentType)
    {
        base.FileName = Path.GetFileName(fullPath);
        Data = data;
    }

    public Task<Stream> OpenFileReadAsync()
       => Task.FromResult<Stream>(File.OpenRead(FullPath));
}
