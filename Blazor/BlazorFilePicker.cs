using Microsoft.JSInterop;
using Microsoft.Maui.Storage;

namespace Avae.Essentials;

/// <summary>
/// File picker backed by a hidden &lt;input type="file"&gt; element. Picked file contents
/// are copied into a temp directory so the returned <see cref="FileResult"/> paths are
/// readable with regular System.IO APIs.
/// PickOptions.FileTypes is platform-keyed and has no browser entry, so it is ignored.
/// </summary>
public sealed class BlazorFilePicker(IJSRuntime js) : IFilePicker
{
    public async Task<FileResult?> PickAsync(PickOptions? options = null)
        => (await PickCoreAsync(
            multiple: false,
            await BlazorEssentials.InitializeAsync(js))).FirstOrDefault();

    private sealed record PickedFile(string Name, string Type, byte[] data);

    public async Task<IEnumerable<FileResult?>> PickMultipleAsync(PickOptions? options = null)
        => await PickCoreAsync(            
            multiple: true,
            await BlazorEssentials.InitializeAsync(js)).ConfigureAwait(false);

    public static async Task<IEnumerable<FileResult>> PickCoreAsync(bool multiple, IJSObjectReference module, int attempt = 0)
    {
        const int maxAttempts = 2;

        try
        {
            var files = await module.InvokeAsync<PickedFile[]>(
                "pickFiles", (string?)null, multiple);

            var pickDirectory = Path.Combine(
                Path.GetTempPath(), "maui-filepicker", Guid.NewGuid().ToString("N"));

            var results = new List<FileResult>(files.Length);
            foreach (var file in files)
            {
                Directory.CreateDirectory(pickDirectory);
                var path = Path.Combine(pickDirectory, file.Name);
                await File.WriteAllBytesAsync(path, file.data).ConfigureAwait(false);

                var contentType = string.IsNullOrEmpty(file.Type)
                    ? "application/octet-stream"
                    : file.Type;

                results.Add(new BlazorFileResult(path, contentType));
            }
            return results;
        }
        catch (JSDisconnectedException) when (attempt < maxAttempts)
        {
            // Circuit dropped — try to re-import once, then give up.
            try
            {
                module = await BlazorEssentials.InvokeCoreAsync(CircuitServiceAccessor.Runtime);
            }
            catch (JSDisconnectedException)
            {
                return Enumerable.Empty<FileResult>();
            }

            return await PickCoreAsync(multiple, module, attempt + 1);
        }
        catch (JSDisconnectedException)
        {
            return Enumerable.Empty<FileResult>();
        }
    }
}