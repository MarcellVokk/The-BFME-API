using System.Drawing;
using System.Runtime.InteropServices;
using The_BFME_API.BFME_Shared;

namespace The_BFME_API.BFME2
{
    internal static class ScreenReader
    {
        public static bool IsMenu1Visible()
        {
            Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen())
            {
                for (var x = 0; x < bitMap.Size.Width; x++)
                {
                    var pixel = bitMap.GetPixel(x, ConfigManager.GetPosFromConfig("ButtonMultiplayer").Y);

                    if (pixel.R >= 150 && pixel.R <= 170 && pixel.G >= 171 && pixel.G <= 191 && pixel.B >= 85 && pixel.B <= 105)
                    {
                        double relativeButtonPosition = (double)x / curentResolution.Width * 100d;
                        if(relativeButtonPosition > 6d && relativeButtonPosition < 7d)
                        {
                            return true;
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        public static bool IsMenuCustomGameLobbyVisible()
        {
            using (Bitmap bitMap = GrabScreen())
            {
                if(bitMap.GetPixel(ConfigManager.GetPosFromConfig("ButtonOpenPlay").X, ConfigManager.GetPosFromConfig("ButtonOpenPlay").Y).GetBrightness() * 100 > 55)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInLobby()
        {
            using (Bitmap bitMap = GrabScreen())
            {
                if (bitMap.GetPixel(ConfigManager.GetPosFromConfig("ButtonOpenPlay").X, ConfigManager.GetPosFromConfig("ButtonOpenPlay").Y).GetBrightness() * 100 > 55)
                {
                    return false;
                }
            }

            return true;
        }

        public static int GetPlayerYLocationOnScreen(bool applyOffHostOffset)
        {
            Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen())
            {
                for (var y = (int)(curentResolution.Height * 0.25d); y < bitMap.Size.Height; y++)
                {
                    var pixel = bitMap.GetPixel(ConfigManager.GetPosFromConfig("TeamButtonXAndSize").X + (applyOffHostOffset ? ConfigManager.GetPosFromConfig("OffHostOffset").X : 0), y);

                    if (pixel.R >= 112 && pixel.R <= 132 && pixel.G >= 235 && pixel.G <= 255 && pixel.B < 10)
                    {
                        return y;
                    }
                }
            }

            return 0;
        }

        public static Tuple<int, int> GetArmyDropdownHeightAndMinYPos(bool applyOffHostOffset)
        {
            using (Bitmap bitMap = GrabScreen())
            {
                int height = -1;
                int yMin = 0;

                for (var y = GetPlayerYLocationOnScreen(applyOffHostOffset); y < bitMap.Size.Height; y++)
                {
                    var pixel = bitMap.GetPixel(ConfigManager.GetPosFromConfig("TeamButtonXAndSize").X + (applyOffHostOffset ? ConfigManager.GetPosFromConfig("OffHostOffset").X : 0), y);

                    if (pixel.R == 106 && pixel.G == 171 && pixel.B == 84)
                    {
                        if (height == -1)
                        {
                            height = 0;
                            yMin = y;
                        }
                        else
                        {
                            return new Tuple<int, int>(height, yMin);
                        }
                    }
                    else if(height != -1)
                    {
                        height++;
                    }
                }
            }

            return new Tuple<int, int>(0, 0);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szDevice;
        }

        public const int MONITOR_DEFAULTTOPRIMARY = 1;

        public static Rectangle GetPrimaryScreenBounds()
        {
            IntPtr desktop = GetDesktopWindow();
            IntPtr monitor = MonitorFromWindow(desktop, MONITOR_DEFAULTTOPRIMARY);

            MONITORINFOEX monitorInfo = new MONITORINFOEX();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);

            if (GetMonitorInfo(monitor, ref monitorInfo))
            {
                RECT rc = monitorInfo.rcMonitor;
                return new Rectangle(rc.Left, rc.Top, rc.Right - rc.Left, rc.Bottom - rc.Top);
            }

            return Rectangle.Empty;
        }

        public static Bitmap GrabScreen()
        {
            Size curentResolution = GameDataManager.GetCurentResolution();

            Bitmap screenshot = new Bitmap(curentResolution.Width, curentResolution.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Graphics gfxScreenshot = Graphics.FromImage(screenshot);

            Rectangle screen = GetPrimaryScreenBounds();

            gfxScreenshot.CopyFromScreen(screen.X, screen.Y, 0, 0, curentResolution, CopyPixelOperation.SourceCopy);

            gfxScreenshot.Dispose();

            return screenshot;
        }
    }
}
