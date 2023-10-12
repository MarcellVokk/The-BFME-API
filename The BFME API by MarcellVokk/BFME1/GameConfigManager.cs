using Microsoft.Win32;

namespace The_BFME_API_by_MarcellVokk.BFME1
{
    public static class GameConfigManager
    {
        public static string GetGameLanguage()
        {
            try
            {
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(@$"SOFTWARE\WOW6432Node\Electronic Arts\EA Games\The Battle for Middle-earth", false);

                if (key != null)
                {
                    string result = key.GetValue("Language") as string;
                    key.Close();
                    return result;
                }
            }
            catch
            {
                return "english";
            }

            return "english";
        }

        public static string GetGameUserDataFolderName()
        {
            try
            {
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(@$"SOFTWARE\WOW6432Node\Electronic Arts\EA Games\The Battle for Middle-earth", false);

                if (key != null)
                {
                    string result = key.GetValue("UserDataLeafName") as string;
                    key.Close();
                    return result;
                }
            }
            catch
            {
                return "My Battle for Middle-earth Files";
            }

            return "My Battle for Middle-earth Files";
        }

        public static bool IsGameInstalled()
        {
            try
            {
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(@$"SOFTWARE\WOW6432Node\Electronic Arts\EA Games\The Battle for Middle-earth", false);

                if (key != null)
                {
                    return true;
                }
            }
            catch
            {
                
            }

            return false;
        }

        public static void SetPlayerSettings(string mapId, int armyId, string username, int color)
        {
            if (armyId == 1)
            {
                armyId = -1;
            }
            else if (armyId == 2)
            {
                armyId = 3;
            }
            else if (armyId == 3)
            {
                armyId = 4;
            }
            else if (armyId == 4)
            {
                armyId = 5;
            }
            else if (armyId == 5)
            {
                armyId = 6;
            }

            Dictionary<string, string> settings = new Dictionary<string, string>
            {
                { "Map", mapId },
                { "PlayerTemplate", armyId.ToString() },
                { "UserName", string.Join("", username.ToCharArray().Select(x => x + "_00")) },
                { "Color", color.ToString() }
            };

            using (StreamWriter sw = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GetGameUserDataFolderName(), "Network.ini")))
            {
                sw.Write(string.Join('\n', settings.Select(x => $"{x.Key} = {x.Value}")));
            }
        }
    }
}
