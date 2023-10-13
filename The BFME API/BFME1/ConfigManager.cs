using System.Drawing;
using The_BFME_API.Logging;

namespace The_BFME_API.BFME1
{
    internal static class ConfigManager
    {
        private static Dictionary<string, Point> Buttons = new Dictionary<string, Point>
        {
            { "ButtonMultiplayer", new Point(700, 1380) },
            { "ButtonNetwork", new Point(1070, 1380) },

            { "ButtonCreateGame", new Point(1070, 1380) },
            { "CurentGamesFirstItem", new Point(1280, 455) },

            { "TeamButtonXAndSize", new Point(1090, 50) },

            { "ButtonStartGame", new Point(1070, 1380) },

            { "MapTopLeft", new Point(1376, 316) },
            { "MapSize", new Point(346, 260) },
            { "NonHostMapSpotOffset", new Point(-21, 151) },

            { "VictoryPixel1", new Point(1139, 719) },
            { "VictoryPixel2", new Point(1150, 719) },
            { "VictoryPixel3", new Point(1168, 719) },
            { "VictoryPixel4", new Point(1177, 719) },
            { "VictoryPixel5", new Point(1213, 719) },
            { "VictoryPixel6", new Point(1231, 719) },
            { "VictoryPixel7", new Point(1258, 719) },
            { "VictoryPixel8", new Point(1268, 719) },
            { "VictoryPixel9", new Point(1294, 719) },
            { "VictoryPixel10", new Point(1305, 719) },
            { "VictoryPixel11", new Point(1333, 719) },
            { "VictoryPixel12", new Point(1342, 719) },
            { "VictoryPixel13", new Point(1362, 719) },
            { "VictoryPixel14", new Point(1378, 719) },
        };

        public static Point GetPosFromConfig(string key, Point? offset = null)
        {
            Size curentResolution = GameDataManager.GetCurentResolution();
            return new Point((int)(Buttons[key].X / 2560d * curentResolution.Width) + (offset is not null ? offset.Value.X : 0), (int)(Buttons[key].Y / 1440d * curentResolution.Height) + (offset is not null ? offset.Value.Y : 0));
        }
    }
}
