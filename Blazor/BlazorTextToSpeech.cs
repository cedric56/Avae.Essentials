using Avae.Essentials;
using Microsoft.Maui.Media;
using Toolbelt.Blazor.SpeechSynthesis;

namespace Avae.Essentials;

internal class BlazorTextToSpeech(CircuitServiceAccessor circuitServiceAccessor) : ITextToSpeech
{
    Dictionary<Locale, SpeechSynthesisVoice> dic = new();

    public async Task<IEnumerable<Locale>> GetLocalesAsync()
    {
        var speechSynthesis = circuitServiceAccessor.GetRequiredService<SpeechSynthesis>();
        return (await speechSynthesis.GetVoicesAsync()).Select(v =>
        {
            var country = CountryResolver.Resolve(v.Lang);
            var locale = (Locale)EssentialsAccessors.CreateLocale(v.Lang ?? string.Empty, country ?? string.Empty, v.Name, v.VoiceIdentity);
            dic.Add(locale, v);
            return locale;
        });
    }

    public async Task SpeakAsync(string text, SpeechOptions? options = null, CancellationToken cancelToken = default)
    {
        var speechSynthesis = circuitServiceAccessor.GetRequiredService<SpeechSynthesis>();
        var utterance = new SpeechSynthesisUtterance()
        {
            Text = text
        };

        if (options != null)
        {
            if (options.Volume.HasValue)
                utterance.Volume = options.Volume.Value;
            if (options.Rate.HasValue)
                utterance.Rate = options.Rate.Value;
            if (options.Pitch.HasValue)
                utterance.Pitch = options.Pitch.Value;
            if (options.Locale != null)
            {
                if (!string.IsNullOrWhiteSpace(options.Locale.Language))
                    utterance.Lang = options.Locale.Language;
                if (dic.TryGetValue(options.Locale!, out var voice))
                    utterance.Voice = voice;
            }
        }

        await speechSynthesis.SpeakAsync(utterance);
    }
}
