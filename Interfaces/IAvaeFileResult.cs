namespace Avae.Essentials;

public interface IAvaeFileResult
{
    Task<Stream> OpenFileStreamAsync();
}
