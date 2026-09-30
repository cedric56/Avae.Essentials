using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using System.Text.Json;

namespace Avae.Essentials;

public partial interface IAvaeShare : IShare
{
    Task RequestAsync(string title, IEnumerable<FileBase> files);
}
