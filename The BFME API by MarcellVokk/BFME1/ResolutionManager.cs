using System.Drawing;

namespace The_BFME_API_by_MarcellVokk.BFME1
{
    public static class ResolutionManager
    {
        public static Size GetResolutionFromSettings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            try
            {
                using (StreamReader sr = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GameConfigManager.GetGameUserDataFolderName(), "Options.ini")))
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

        public static void SetResolution(Size newResolution)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            try
            {
                using (StreamReader sr = new StreamReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GameConfigManager.GetGameUserDataFolderName(), "Options.ini")))
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
                settings["Resolution"] = $"{newResolution.Width} {newResolution.Height}";
            }
            else
            {
                settings.Add("Resolution", $"{newResolution.Width} {newResolution.Height}");
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GameConfigManager.GetGameUserDataFolderName(), "Options.ini")))
            {
                sw.Write(string.Join('\n', settings.Select(x => $"{x.Key} = {x.Value}")));
            }
        }
    }
}
