using Append.Blazor.WebShare;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using System.Text.Json.Serialization;

namespace Avae.Essentials;

internal partial class BlazorShare(CircuitServiceAccessor circuitServiceAccessor) : IAvaeShare
{
    public async Task RequestAsync(ShareTextRequest request)
    {
        var service = circuitServiceAccessor.GetRequiredService<IWebShareService>();
        if (await service.IsSupportedAsync())
            await service.ShareAsync(request.Title ?? string.Empty, request.Text ?? string.Empty, request.Uri ?? string.Empty);
    }

    public Task RequestAsync(ShareFileRequest request)
    {
        return RequestAsync(request.Title ?? string.Empty, request.File is null ? [] : [request.File]);
    }

    public Task RequestAsync(ShareMultipleFilesRequest request)
    {
        return RequestAsync(request.Title ?? string.Empty, request.Files ?? []);
    }

    public async Task RequestAsync(string title, IEnumerable<FileBase> files)
    {
        if (!files.All(file => file is BlazorFileResult))
            throw new InvalidOperationException("Files must have been loaded from library");

        var service = circuitServiceAccessor.GetRequiredService<IWebShareService>();
        if (await service.IsSupportedAsync())
            await service.ShareAsync(new ShareDataEx()
            {
                Title = title,
                Files = [.. files.OfType<BlazorFileResult>()
                            .Where(f => f.JSReference != null) //TODO
                            .Select(f => f.JSReference!)]
            });
    }

    class ShareDataEx : ShareData
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IJSObjectReference[]? Files { get; set; }
    }
}
