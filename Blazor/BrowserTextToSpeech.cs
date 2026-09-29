using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;
using System.Runtime.Versioning;

namespace Avae.Essentials;

/// <summary>Text to speech backed by the Web Speech API (speechSynthesis).</summary>
public class BrowserTextToSpeech(IJSRuntime js) : ITextToSpeech
{
    public async Task<IEnumerable<Locale>> GetLocalesAsync()
	{
		// Locale has no public constructor, so voices cannot be surfaced through the
		// MAUI Locale type. SpeakAsync honors SpeechOptions.Volume and Pitch; a
		// specific voice can only be selected by the browser's language matching.
		return [];
	}

	public async Task SpeakAsync(string text, SpeechOptions? options = null, CancellationToken cancelToken = default)
	{
		cancelToken.ThrowIfCancellationRequested();

		using var registration = cancelToken.CanBeCanceled
			? cancelToken.Register(() => { BrowserEssentials.SpeechCancel(js); })
			: default;

		try
		{
			await BrowserEssentials.SpeakAsync(
                js,
                text,
				options?.Locale?.Language,
				options?.Pitch ?? -1,
				-1,
				options?.Volume ?? -1).ConfigureAwait(false);
		}
		catch (Exception ex) when (ex.Message.Contains("unsupported", StringComparison.OrdinalIgnoreCase))
		{
			throw new FeatureNotSupportedException("The Web Speech API is not available in this browser.");
		}
		cancelToken.ThrowIfCancellationRequested();
	}
}
