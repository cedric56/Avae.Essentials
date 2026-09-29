using Avalonia.Controls.Maui.Essentials;
using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using System.Text.Json;

namespace Avae.Essentials;

/// <summary>
/// Share backed by the Web Share API (navigator.share). Requires a secure context
/// and, in most browsers, a user gesture. File sharing uses Web Share Level 2.
/// </summary>
public class BlazorShare(IJSRuntime js) : IAvaeShare
{
    public async Task RequestAsync(ShareTextRequest request)
    {
        if (BlazorEssentials.IsWasm)
        {
            if (!await BlazorEssentialsInterop.InvokeWithRetryAsync<bool>(js, "shareIsSupported"))
                throw new FeatureNotSupportedException("The Web Share API is not available in this browser.");
            await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(
                BlazorEssentials.IsWasm ? js : CircuitServiceAccessor.Runtime,
                "share", request.Title, request.Text, request.Uri).ConfigureAwait(false);
        }
        else
        {
            if(await (await BlazorEssentials.InvokeCoreAsync(CircuitServiceAccessor.Runtime)).InvokeAsync<bool>("shareIsSupported").ConfigureAwait(false))
                throw new FeatureNotSupportedException("The Web Share API is not available in this browser.");
            await (await BlazorEssentials.InvokeCoreAsync(CircuitServiceAccessor.Runtime)).InvokeVoidAsync(
                "share", request.Title, request.Text, request.Uri).ConfigureAwait(false);
        }
    }

    public Task RequestAsync(ShareFileRequest request) =>
        ShareFilesAsync(request.Title, request.File is null ? [] : [request.File]);

    public Task RequestAsync(ShareMultipleFilesRequest request) =>
        ShareFilesAsync(request.Title, request.Files ?? []);

    public Task RequestAsync(string title, IEnumerable<FileBase> files)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentNullException.ThrowIfNull(files, nameof(files));

        // Convert the enumerable to a list to avoid multiple enumeration and get accurate count
        var shareFiles = new List<ShareFile>(files.Count());

        foreach (var file in files)
        {
            // Check if the file is an Avalonia-specific file result
            if (file is AvaloniaFileResult f)
                // Wrap it with the Avalonia adapter for proper handling
                shareFiles.Add(new AvaeShareFile(f));
            else if (file is BlazorFileResult b)
                shareFiles.Add(new ShareFile(b.FullPath, b.ContentType));
            else
                // Use standard MAUI ShareFile for regular files
                shareFiles.Add(new ShareFile(file));
        }

        // Execute the native share request with the converted files
        return RequestAsync(new ShareMultipleFilesRequest()
        {
            Title = title,
            Files = shareFiles
        });
    }

    async Task ShareFilesAsync(string? title, IReadOnlyList<ShareFile> files)
    {
        if (!await BlazorEssentialsInterop.InvokeWithRetryAsync<bool>(js, "shareIsSupported"))
            throw new FeatureNotSupportedException("The Web Share API is not available in this browser.");
        if (files.Count == 0)
            throw new ArgumentException("No files were provided to share.");

        var names = new string[files.Count];
        var types = new string[files.Count];
        var contents = new string[files.Count];
        for (var i = 0; i < files.Count; i++)
        {
            names[i] = files[i].FileName;
            types[i] = files[i].ContentType ?? string.Empty;
            contents[i] = Convert.ToBase64String(await File.ReadAllBytesAsync(files[i].FullPath).ConfigureAwait(false));
        }

        try
        {
            if (BlazorEssentials.IsWasm)
            {
                await BlazorEssentialsInterop.InvokeVoidWithRetryAsync(
                    js,
                    "shareFiles",
                    title,
                    JsonSerializer.Serialize(names),
                    JsonSerializer.Serialize(types),
                    JsonSerializer.Serialize(contents)).ConfigureAwait(false);
            }
            else
            {
                await (await BlazorEssentials.InvokeCoreAsync(CircuitServiceAccessor.Runtime)).
                    InvokeVoidAsync(
                    "shareFiles",
                    title,
                    JsonSerializer.Serialize(names),
                    JsonSerializer.Serialize(types),
                    JsonSerializer.Serialize(contents)).ConfigureAwait(false);

            }
        }
        catch (Exception ex) when (ex.Message.Contains("unsupported", StringComparison.OrdinalIgnoreCase))
        {
            throw new FeatureNotSupportedException("This browser cannot share files via the Web Share API.");
        }
    }
}

