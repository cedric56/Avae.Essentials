using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Avae.Essentials;

partial class AvaePhoneDialer : IPhoneDialer
{
    void ValidateOpen(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentNullException(nameof(number));

        if (!IsSupported)
            throw new FeatureNotSupportedException();
    }
}

partial class AvaePhoneDialer : IPhoneDialer
{
    public bool IsSupported =>
        true;

    public async void Open(string number)
    {
        ValidateOpen(number);

        //if (ApiInformation.IsTypePresent("Windows.ApplicationModel.Calls.PhoneCallManager"))
        //	PhoneCallManager.ShowPhoneCallUI(number, string.Empty);
        //else
        await Launcher.OpenAsync($"tel:{number}");
    }
}
