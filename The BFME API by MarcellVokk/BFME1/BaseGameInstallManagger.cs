using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace The_BFME_API_by_MarcellVokk.BFME1
{
    public static class BaseGameInstallManagger
    {
        public static string GameDirectory
        {
            get
            {
                try
                {
                    RegistryKey? key = Registry.LocalMachine.OpenSubKey(@$"SOFTWARE\WOW6432Node\Electronic Arts\EA Games\The Battle for Middle-earth", false);

                    if (key != null)
                    {
                        string result = key.GetValue("InstallPath") as string;
                        key.Close();
                        return result;
                    }
                }
                catch
                {
                    return "C:/";
                }

                return "C:/";
            }
        }
    }
}
