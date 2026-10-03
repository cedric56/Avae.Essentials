using System.IO;
using System.Threading.Tasks;

namespace Avae.Essentials;

public interface IAvaeFileResult
{
    Task<Stream> OpenFileStreamAsync();
}
