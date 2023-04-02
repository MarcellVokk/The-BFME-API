using System.Drawing;
using System.Windows.Forms;

namespace The_BFME_API_by_MarcellVokk.BFME1
{
    public static class ConfigManager
    {
        private static Dictionary<string, string> CurentConfig = new Dictionary<string, string>();
        private static Dictionary<string, Point[]> CurentMapSpotConfig = new Dictionary<string, Point[]>();

        public static bool IsCurentGameResolutionSupported()
        {
            Size resolution = ResolutionManager.GetResolutionFromSettings();

            return IsResolutionSupported(resolution);
        }

        public static void SwitchToClosestSupportedResolution()
        {
            ResolutionManager.SetResolution(GetClosestSupportedResolutionToCurent());
        }

        public static Size GetClosestSupportedResolutionToCurent()
        {
            Size curentUnsuportedResolution = ResolutionManager.GetResolutionFromSettings();

            double difference = double.MaxValue;
            Size newSupportedResolution = new Size(0, 0);

            foreach (Size resolution in GetSupportedResolutions())
            {
                if (Math.Abs(resolution.Width - curentUnsuportedResolution.Width) + Math.Abs(resolution.Height - curentUnsuportedResolution.Height) < difference)
                {
                    difference = Math.Abs(resolution.Width - curentUnsuportedResolution.Width) + Math.Abs(resolution.Height - curentUnsuportedResolution.Height);
                    newSupportedResolution = resolution;
                }
            }

            return newSupportedResolution;
        }

        public static bool IsResolutionSupported(Size resolution)
        {
            return Directory.Exists($@".\BFME1\bfme_api_resources\{resolution.Width}x{resolution.Height}");
        }

        public static List<Size> GetSupportedResolutions()
        {
            List<Size> result = new List<Size>();

            foreach(string? dir in Directory.GetDirectories($@".\BFME1\bfme_api_resources").Select(x => Path.GetFileNameWithoutExtension(x)))
            {
                if(dir != null && dir.Split('x').Length > 1)
                {
                    if(int.TryParse(dir.Split('x')[0], out int width) && int.TryParse(dir.Split('x')[1], out int height))
                    {
                        result.Add(new Size(width, height));
                    }
                }
            }

            return result;
        }

        public static void ReloadConfig()
        {
            Dictionary<string, string> newConfig = new Dictionary<string, string>();

            if (IsCurentGameResolutionSupported())
            {
                Size curentResolution = ResolutionManager.GetResolutionFromSettings();
                string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

                try
                {
                    using (StreamReader sr = new StreamReader($@".\BFME1\bfme_api_resources\{resolutionId}\config.conf"))
                    {
                        string configRaw = sr.ReadToEnd();

                        foreach (string entry in configRaw.Split('\n'))
                        {
                            if (entry.Split(" = ").Length > 1 && !newConfig.ContainsKey(entry.Split(" = ")[0]))
                            {
                                newConfig.Add(entry.Split(" = ")[0], entry.Split(" = ")[1]);
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            CurentConfig = newConfig;
        }

        public static void ReloadMapSpotConfig()
        {
            Dictionary<string, Point[]> newMapSpotConfig = new Dictionary<string, Point[]>();

            if (IsCurentGameResolutionSupported())
            {
                try
                {
                    using (StreamReader sr = new StreamReader($@".\BFME1\bfme_api_resources\map_spots.conf"))
                    {
                        string configRaw = sr.ReadToEnd();

                        string curentMap = "";
                        List<Point> curentMapSpots = new List<Point>();

                        foreach (string entry in configRaw.Split('\n'))
                        {
                            if(entry.Contains("map"))
                            {
                                curentMap = entry.Replace("\r", "");
                            }
                            else if(entry.Split(' ').Length == 2)
                            {
                                curentMapSpots.Add(new Point(int.Parse(entry.Split(' ')[0]), int.Parse(entry.Split(' ')[1])));
                            }
                            else
                            {
                                newMapSpotConfig.Add(curentMap, curentMapSpots.ToArray());
                                curentMapSpots.Clear();
                            }
                        }

                        if(curentMapSpots.Count > 0)
                        {
                            newMapSpotConfig.Add(curentMap.ToString(), curentMapSpots.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            CurentMapSpotConfig = newMapSpotConfig;
        }

        public static Point GetPosFromConfig(string key)
        {
            string data = CurentConfig[key];
            return new Point(int.Parse(data.Split(' ')[0]), int.Parse(data.Split(' ')[1]));
        }

        public static int GetIntFromConfig(string key)
        {
            return int.Parse(CurentConfig[key]);
        }

        public static Point GetMapSpotFromConfig(string mapId, int spotId)
        {
            Point ingameMapSize = ConfigManager.GetPosFromConfig("MapSize");
            Point ingameMapTopLeft = ConfigManager.GetPosFromConfig("MapTopLeft");

            double scaleFactor_x = ingameMapSize.X / 346d;
            double scaleFactor_y = ingameMapSize.Y / 260d;

            Point result = CurentMapSpotConfig[mapId.Replace("_20nm_", "_")][spotId - 1];

            return new Point((int)(result.X * scaleFactor_x) + ingameMapTopLeft.X, (int)(result.Y * scaleFactor_y) + ingameMapTopLeft.Y);
        }
    }
}
