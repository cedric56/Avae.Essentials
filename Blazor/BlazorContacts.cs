using Microsoft.JSInterop;
using Microsoft.Maui.ApplicationModel.Communication;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Avae.Essentials;

/// <summary>Contacts are not accessible from the browser (the Contact Picker API is not broadly available).</summary>
public class BlazorContacts : IContacts
{
    public async Task<IEnumerable<Contact>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await BlazorEssentials.Module.InvokeAsync<string>("getAllContactsAsync", true);
        return Parse(result);
    }

    public async Task<Contact?> PickContactAsync()
    {
        var result = await BlazorEssentials.Module.InvokeAsync<string>("getAllContactsAsync", false);
        return Parse(result).FirstOrDefault();
    }

    private static List<Contact> Parse(string result)
    {
        if (!string.IsNullOrWhiteSpace(result))
        {
            var contacts = new List<Contact>();

            var results = JsonSerializer.Deserialize(result, BlazorJsonSerializerContext.Default.ListContactsResponseInterop);

            if (results is null)
            {
                return contacts;
            }

            foreach (var item in results)
            {
                var contact = new Contact()
                {
                    GivenName = item.Name?.FirstOrDefault() ?? string.Empty,
                    Emails = item.Email?.Select(e => new ContactEmail { EmailAddress = e }).ToList() ?? [],
                    Phones = item.Tel?.Select(e => new ContactPhone { PhoneNumber = e }).ToList() ?? [],
                };
                contacts.Add(contact);
            }
            return contacts;
        }
        return [];
    }
}

public class ContactsResponseInterop
{
    [JsonPropertyName("name")]
    public List<string>? Name { get; set; }

    [JsonPropertyName("email")]
    public List<string>? Email { get; set; }

    [JsonPropertyName("tel")]
    public List<string>? Tel { get; set; }

    [JsonPropertyName("address")]
    public List<string>? Address { get; set; }
}