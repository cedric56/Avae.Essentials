using Microsoft.Maui.Devices.Sensors;
namespace Avae.Essentials;

class AvaeGeocoding : IGeocoding
{
    public async Task<IEnumerable<Placemark>> GetPlacemarksAsync(double latitude, double longitude)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Location>> GetLocationsAsync(string address)
    {
        throw new NotImplementedException();
    }
}