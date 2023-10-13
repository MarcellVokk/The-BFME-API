using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
