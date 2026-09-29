using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.Runtime.Versioning;
using System.Text.Json;

namespace Avae.Essentials;

/// <summary>
/// Share backed by the Web Share API (navigator.share). Requires a secure context
/// and, in most browsers, a user gesture. File sharing uses Web Share Level 2.
/// </summary>
public class BrowserShare(IJSRuntime js) : IShare
{
    public async Task RequestAsync(ShareTextRequest request)
	{
		if (!BrowserEssentials.ShareIsSupported(js))
			throw new FeatureNotSupportedException("The Web Share API is not available in this browser.");
		await BrowserEssentials.ShareAsync(js, request.Title, request.Text, request.Uri).ConfigureAwait(false);
	}

	public Task RequestAsync(ShareFileRequest request) =>
		ShareFilesAsync(request.Title, request.File is null ? [] : [request.File]);

	public Task RequestAsync(ShareMultipleFilesRequest request) =>
		ShareFilesAsync(request.Title, request.Files ?? []);

	async Task ShareFilesAsync(string? title, IReadOnlyList<ShareFile> files)
	{
		if (!BrowserEssentials.ShareIsSupported(js))
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
			await BrowserEssentials.ShareFilesAsync(
				js,
				title,
				JsonSerializer.Serialize(names),
				JsonSerializer.Serialize(types),
				JsonSerializer.Serialize(contents)).ConfigureAwait(false);
		}
		catch (Exception ex) when (ex.Message.Contains("unsupported", StringComparison.OrdinalIgnoreCase))
		{
			throw new FeatureNotSupportedException("This browser cannot share files via the Web Share API.");
		}
	}
}
