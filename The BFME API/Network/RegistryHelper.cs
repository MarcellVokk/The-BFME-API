using Microsoft.Win32;

namespace The_BFME_API.Network
{
    public static class RegistryHelper
    {
        public static void DisableNewNetworkPopup()
        {
            Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Network\NewNetworkWindowOff");
        }
    }
}
