using Microsoft.Maui.Storage;

namespace Avae.Essentials;

internal class BlazorFileResult : FileResult
{
    public BlazorFileResult(string fullPath, string contentType)
        : base(fullPath, contentType)
    {        
        base.FileName = Path.GetFileName(fullPath);
    }
}
