using Microsoft.Phone.Net.NetworkInformation;

namespace RestAssure.Settings
{
    public static class RestAssureSettings
    {
        public const string Email = "info@nattarolabs.se";
        public const string Url = "http://www.nattarolabs.se";

        public static bool IsInternetAvailable()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            return true;
        }
    }
}