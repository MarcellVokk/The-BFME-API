using Microsoft.Win32;
using System.Drawing;
using The_BFME_API.Logging;

namespace The_BFME_API.BFME2
{
    internal static class GameDataManager
    {
        public static string GetGameInstallDirectory()
        {
            try
            {
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(@$"SOFTWARE\WOW6432Node\Electronic Arts\Electronic Arts\The Battle for Middle-earth II", false);

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
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(@$"SOFTWARE\WOW6432Node\Electronic Arts\Electronic Arts\The Battle for Middle-earth II", false);

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
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(@$"SOFTWARE\WOW6432Node\Electronic Arts\Electronic Arts\The Battle for Middle-earth II", false);

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

            return "My Battle for Middle-earth II Files";
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

        public static void SetLaunchSettings(string mapId, int armyId, int heroId, string username, int color, int commandPointFactor, int initialResources, bool allowCustomHeroes, bool allowRingHeroes)
        {
            Logger.LogDiagnostic("Updating NetworkPref.ini...", "GameDataManager");

            Dictionary<string, string> settings = new Dictionary<string, string>
            {
                { "Rts:Map", mapId },
                { "Rts:Hero", heroId.ToString() },
                { "Rts:PlayerTemplate", armyId.ToString() },
                { "UserName", string.Join("", username.ToCharArray().Select(x => x + "_00")) },
                { "Rts:Color", color.ToString() },
                { "Rts:Rules", $"{(allowCustomHeroes ? "1" : "0")} 0 {(allowRingHeroes ? "1" : "0")} {commandPointFactor} {initialResources} -1 -1 -1 -1 -1 " }
            };

            using (StreamWriter sw = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GetGameUserDataFolderName(), "NetworkPref.ini")))
            {
                sw.Write(string.Join('\n', settings.Select(x => $"{x.Key} = {x.Value}")));
            }

            Logger.LogDiagnostic("Updating NetworkPref.ini... DONE!", "GameDataManager");
        }
    }
}
