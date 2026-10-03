using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Avae.Essentials;

[JsonSerializable(typeof(ContactsResponseInterop))]
[JsonSerializable(typeof(List<ContactsResponseInterop>))]
[JsonSerializable(typeof(PlacemarkResponseInterop))]
[JsonSerializable(typeof(IEnumerable<NominatimResponse>))]
//[JsonSerializable(typeof(GeolocationResultInterop))]
//[JsonSerializable(typeof(TextToSpeechResponseInterop))]
//[JsonSerializable(typeof(IEnumerable<TextToSpeechResponseInterop>))]
//[JsonSerializable(typeof(GeolocationReadingResultInterop))]
public partial class BlazorJsonSerializerContext : JsonSerializerContext
{
}
