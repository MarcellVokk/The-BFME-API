using Microsoft.Win32;
using System.Drawing;
using The_BFME_API.Logging;

namespace The_BFME_API.BFME1
{
    internal static class GameDataManager
    {
        public static string GetGameInstallDirectory()
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
                
            }

            return "My Battle for Middle-earth Files";
        }

        public static Size GetCurentResolution()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            try
            {
                using (StreamReader sr = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GameDataManager.GetGameUserDataFolderName(), "Options.ini")))
                {
                    foreach (string entry in sr.ReadToEnd().Split('\n'))
                    {
                        if (entry != "")
                        {
                            settings.Add(entry.Split(" = ")[0], entry.Split(" = ")[1]);
                        }
                    }
                }
            }
            catch
            {

            }

            if (settings.ContainsKey("Resolution"))
            {
                return new Size(int.Parse(settings["Resolution"].Split(' ')[0]), int.Parse(settings["Resolution"].Split(' ')[1]));
            }

            return new Size(0, 0);
        }

        public static void SetPlayerSettings(string mapId, int armyId, string username, int color)
        {
            Logger.LogDiagnostic("Updating Network.ini...", "GameDataManager");

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

            Logger.LogDiagnostic("Updating Network.ini... DONE!", "GameDataManager");
        }
    }
}
