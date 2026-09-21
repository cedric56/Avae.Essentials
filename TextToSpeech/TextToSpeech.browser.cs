using Microsoft.Maui.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Avae.Everywhere
{
    partial class TextToSpeechImplementation : ITextToSpeech
    {

        private static Dictionary<string, string> localeToCountry = new Dictionary<string, string>
{
    { "af-ZA", "South Africa" },
    { "am-ET", "Ethiopia" },
    { "ar-AE", "United Arab Emirates" },
    { "ar-BH", "Bahrain" },
    { "ar-DZ", "Algeria" },
    { "ar-EG", "Egypt" },
    { "ar-IQ", "Iraq" },
    { "ar-JO", "Jordan" },
    { "ar-KW", "Kuwait" },
    { "ar-LB", "Lebanon" },
    { "ar-LY", "Libya" },
    { "ar-MA", "Morocco" },
    { "ar-OM", "Oman" },
    { "ar-QA", "Qatar" },
    { "ar-SA", "Saudi Arabia" },
    { "ar-SD", "Sudan" },
    { "ar-SY", "Syria" },
    { "ar-TN", "Tunisia" },
    { "ar-YE", "Yemen" },
    { "az-AZ", "Azerbaijan" },
    { "be-BY", "Belarus" },
    { "bg-BG", "Bulgaria" },
    { "bn-BD", "Bangladesh" },
    { "bn-IN", "India" },
    { "bs-BA", "Bosnia and Herzegovina" },
    { "ca-ES", "Spain" },
    { "cs-CZ", "Czech Republic" },
    { "cy-GB", "United Kingdom" },
    { "da-DK", "Denmark" },
    { "de-AT", "Austria" },
    { "de-CH", "Switzerland" },
    { "de-DE", "Germany" },
    { "de-LI", "Liechtenstein" },
    { "de-LU", "Luxembourg" },
    { "el-CY", "Cyprus" },
    { "el-GR", "Greece" },
    { "en-AU", "Australia" },
    { "en-BZ", "Belize" },
    { "en-CA", "Canada" },
    { "en-CB", "Caribbean" },
    { "en-GB", "United Kingdom" },
    { "en-IE", "Ireland" },
    { "en-IN", "India" },
    { "en-JM", "Jamaica" },
    { "en-NZ", "New Zealand" },
    { "en-PH", "Philippines" },
    { "en-TT", "Trinidad and Tobago" },
    { "es-US", "United States" },
    { "en-US", "United States" },
    { "en-ZA", "South Africa" },
    { "en-ZW", "Zimbabwe" },
    { "es-AR", "Argentina" },
    { "es-BO", "Bolivia" },
    { "es-CL", "Chile" },
    { "es-CO", "Colombia" },
    { "es-CR", "Costa Rica" },
    { "es-DO", "Dominican Republic" },
    { "es-EC", "Ecuador" },
    { "es-ES", "Spain" },
    { "es-GT", "Guatemala" },
    { "es-HN", "Honduras" },
    { "es-MX", "Mexico" },
    { "es-NI", "Nicaragua" },
    { "es-PA", "Panama" },
    { "es-PE", "Peru" },
    { "es-PR", "Puerto Rico" },
    { "es-PY", "Paraguay" },
    { "es-SV", "El Salvador" },
    { "es-UY", "Uruguay" },
    { "es-VE", "Venezuela" },
    { "et-EE", "Estonia" },
    { "eu-ES", "Spain" },
    { "fa-IR", "Iran" },
    { "fi-FI", "Finland" },
    { "fo-FO", "Faroe Islands" },
    { "fr-BE", "Belgium" },
    { "fr-CA", "Canada" },
    { "fr-CH", "Switzerland" },
    { "fr-FR", "France" },
    { "fr-LU", "Luxembourg" },
    { "fr-MC", "Monaco" },
    { "gl-ES", "Spain" },
    { "gu-IN", "India" },
    { "he-IL", "Israel" },
    { "hi-IN", "India" },
    { "hr-BA", "Bosnia and Herzegovina" },
    { "hr-HR", "Croatia" },
    { "hu-HU", "Hungary" },
    { "hy-AM", "Armenia" },
    { "id-ID", "Indonesia" },
    { "is-IS", "Iceland" },
    { "it-CH", "Switzerland" },
    { "it-IT", "Italy" },
    { "ja-JP", "Japan" },
    { "ka-GE", "Georgia" },
    { "kk-KZ", "Kazakhstan" },
    { "kn-IN", "India" },
    { "ko-KR", "South Korea" },
    { "kok-IN", "India" },
    { "ky-KG", "Kyrgyzstan" },
    { "lt-LT", "Lithuania" },
    { "lv-LV", "Latvia" },
    { "mi-NZ", "New Zealand" },
    { "mk-MK", "North Macedonia" },
    { "ml-IN", "India" },
    { "mn-MN", "Mongolia" },
    { "mr-IN", "India" },
    { "ms-BN", "Brunei Darussalam" },
    { "ms-MY", "Malaysia" },
    { "mt-MT", "Malta" },
    { "nb-NO", "Norway" },
    { "nl-BE", "Belgium" },
    { "nl-NL", "Netherlands" },
    { "nn-NO", "Norway" },
    { "pa-IN", "India" },
    { "pl-PL", "Poland" },
    { "pt-BR", "Brazil" },
    { "pt-PT", "Portugal" },
    { "ro-RO", "Romania" },
    { "ru-RU", "Russia" },
    { "sa-IN", "India" },
    { "sk-SK", "Slovakia" },
    { "sl-SI", "Slovenia" },
    { "sq-AL", "Albania" },
    { "sr-Cyrl-BA", "Bosnia and Herzegovina" },
    { "sr-Cyrl-CS", "Serbia and Montenegro" },
    { "sr-Cyrl-ME", "Montenegro" },
    { "sr-Cyrl-RS", "Serbia" },
    { "sr-Latn-BA", "Bosnia and Herzegovina" },
    { "sr-Latn-CS", "Serbia and Montenegro" },
    { "sr-Latn-ME", "Montenegro" },
    { "sr-Latn-RS", "Serbia" },
    { "sv-FI", "Finland" },
    { "sv-SE", "Sweden" },
    { "sw-KE", "Kenya" },
    { "syr-SY", "Syria" },
    { "ta-IN", "India" },
    { "te-IN", "India" },
    { "th-TH", "Thailand" },
    { "tr-TR", "Turkey" },
    { "tt-RU", "Russia" },
    { "uk-UA", "Ukraine" },
    { "ur-PK", "Pakistan" },
    { "uz-Cyrl-UZ", "Uzbekistan" },
    { "uz-Latn-UZ", "Uzbekistan" },
    { "vi-VN", "Vietnam" },
    { "xh-ZA", "South Africa" },
    { "zh-CN", "China" },
    { "zh-HK", "Hong Kong" },
    { "zh-MO", "Macau" },
    { "zh-SG", "Singapore" },
    { "zh-TW", "Taiwan" },
    { "zu-ZA", "South Africa" }
};

        [JSImport("textToSpeechInterop.speak", "essentials")]
        internal static partial void Speak(string text);

        [JSImport("textToSpeechInterop.speakWithOptions", "essentials")]
        internal static partial void SpeakWithOptions(string text, string lang, string voice, float? volume, float? pitch);

        [JSImport("textToSpeechInterop.getVoices", "essentials")]
        internal static partial Task<string> GetVoices();

        public async Task<IEnumerable<Locale>> GetLocalesAsync()
        {
            var json = await GetVoices();
            var voices = JsonSerializer.Deserialize(json, AvaeJsonSerializerContext.Default.IEnumerableTextToSpeechResponseInterop);
            if (voices == null)
                return [];
            return [.. voices.Select(ToLocale)];
        }

        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        [return: UnsafeAccessorType("Microsoft.Maui.Media.Locale, Microsoft.Maui.Essentials")]
        extern static object CreateClass(string language, string country, string name, string id);


        private Locale ToLocale(TextToSpeechResponseInterop r)
        {
            localeToCountry.TryGetValue(r.Lang ?? "en", out string? country);

            var locale = (Locale)CreateClass(r.Lang ?? "en", country ?? string.Empty, r.Name ?? string.Empty, r.VoiceURI ?? string.Empty);
            return locale;
        }

        public Task SpeakAsync(string text, SpeechOptions? options = null, CancellationToken cancelToken = default)
        {
            if (options == null)
            {
                Speak(text);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(options.Locale?.Language))
                    throw new ArgumentException("Locale language must be specified.", nameof(options.Locale));

                SpeakWithOptions(text,
                options.Locale.Language,
                options.Locale.Id,
                options.Volume,
                options.Pitch);
            }

            return Task.CompletedTask;
        }
    }

    [JsonSerializable(typeof(TextToSpeechResponseInterop))]
    [JsonSerializable(typeof(IEnumerable<TextToSpeechResponseInterop>))]
    internal partial class AvaeJsonSerializerContext : JsonSerializerContext
    {
    }

    internal class TextToSpeechResponseInterop
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("lang")]
        public string? Lang { get; set; }

        [JsonPropertyName("default")]
        public bool Default { get; set; }

        [JsonPropertyName("voiceURI")]
        public string? VoiceURI { get; set; }

        [JsonPropertyName("localService")]
        public bool LocalService { get; set; }
    }
}