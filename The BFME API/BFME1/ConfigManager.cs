using System.Drawing;
using The_BFME_API.Logging;

namespace The_BFME_API.BFME1
{
    internal static class ConfigManager
    {
        private static Dictionary<string, Point> Buttons = new Dictionary<string, Point>();
        private static Dictionary<string, Point[]> Spots = new Dictionary<string, Point[]>();

        public static void Load()
        {
            Logger.LogDiagnostic("Loading config...", "ConfigManager");

            Buttons.Clear();
            using (StreamReader sr = new StreamReader($@".\BFME1\bfme_api_resources\buttons.conf"))
            {
                string configRaw = sr.ReadToEnd();

                foreach (string entry in configRaw.Split('\n'))
                {
                    if (entry.Split(" = ").Length > 1 && !Buttons.ContainsKey(entry.Split(" = ")[0]))
                    {
                        Buttons.Add(entry.Split(" = ")[0], new Point(int.Parse(entry.Split(" = ")[1].Split(' ')[0]), int.Parse(entry.Split(" = ")[1].Split(' ')[1])));
                    }
                }
            }

            Spots.Clear();
            using (StreamReader sr = new StreamReader($@".\BFME1\bfme_api_resources\map_spots.conf"))
            {
                string configRaw = sr.ReadToEnd();

                string curentMap = "";
                List<Point> curentMapSpots = new List<Point>();

                foreach (string entry in configRaw.Split('\n'))
                {
                    if (entry.Contains("map"))
                    {
                        curentMap = entry.Replace("\r", "");
                    }
                    else if (entry.Split(' ').Length == 2)
                    {
                        curentMapSpots.Add(new Point(int.Parse(entry.Split(' ')[0]), int.Parse(entry.Split(' ')[1])));
                    }
                    else
                    {
                        Spots.Add(curentMap, curentMapSpots.ToArray());
                        curentMapSpots.Clear();
                    }
                }

                if (curentMapSpots.Count > 0)
                {
                    Spots.Add(curentMap.ToString(), curentMapSpots.ToArray());
                }
            }

            Logger.LogDiagnostic("Loading config... DONE!", "ConfigManager");
        }

        public static Point GetPosFromConfig(string key, Point? offset = null)
        {
            Size curentResolution = GameDataManager.GetCurentResolution();
            return new Point((int)(Buttons[key].X / 2560d * curentResolution.Width) + (offset != null ? offset.Value.X : 0), (int)(Buttons[key].Y / 1440d * curentResolution.Height) + (offset != null ? offset.Value.Y : 0));
        }

        public static Point GetMapSpotFromConfig(string mapId, int spotId)
        {
            Point ingameMapSize = GetPosFromConfig("MapSize");
            Point ingameMapTopLeft = GetPosFromConfig("MapTopLeft");

            double scaleFactor_x = ingameMapSize.X / 346d;
            double scaleFactor_y = ingameMapSize.Y / 260d;

            Point result = Spots[mapId.Replace("_20nm_", "_")][spotId - 1];

            return new Point((int)(result.X * scaleFactor_x) + ingameMapTopLeft.X, (int)(result.Y * scaleFactor_y) + ingameMapTopLeft.Y);
        }
    }
}
