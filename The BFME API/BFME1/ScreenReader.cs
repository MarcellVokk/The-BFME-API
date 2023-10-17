using System.Drawing;
using System.Runtime.InteropServices;
using The_BFME_API.BFME_Shared;

namespace The_BFME_API.BFME1
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
                    if ((bitMap.GetPixel(x, ConfigManager.GetPosFromConfig("ButtonMultiplayer").Y).GetBrightness() * 100) > 25)
                    {
                        double relativeButtonPosition = (double)x / curentResolution.Width * 100d;

                        if(relativeButtonPosition > 5d && relativeButtonPosition < 6.5d)
                        {
                            return true;
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        public static bool IsMenu2Visible()
        {
            Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen())
            {
                for (var x = 0; x < bitMap.Size.Width; x++)
                {
                    if ((bitMap.GetPixel(x, ConfigManager.GetPosFromConfig("ButtonMultiplayer").Y).GetBrightness() * 100) > 25)
                    {
                        double relativeButtonPosition = (double)x / curentResolution.Width * 100d;

                        if (relativeButtonPosition > 18d && relativeButtonPosition < 21d)
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
            Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen())
            {
                for (var x = 0; x < bitMap.Size.Width; x++)
                {
                    if ((bitMap.GetPixel(x, ConfigManager.GetPosFromConfig("ButtonMultiplayer").Y).GetBrightness() * 100) > 25)
                    {
                        double relativeButtonPosition = (double)x / curentResolution.Width * 100d;

                        if (relativeButtonPosition > 0d && relativeButtonPosition < 2d)
                        {
                            return true;
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        public static bool IsInLobby()
        {
            Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen())
            {
                for (var x = 0; x < bitMap.Size.Width; x++)
                {
                    if ((bitMap.GetPixel(x, ConfigManager.GetPosFromConfig("ButtonMultiplayer").Y).GetBrightness() * 100) > 25)
                    {
                        double relativeButtonPosition = (double)x / curentResolution.Width * 100d;

                        if (relativeButtonPosition > 30d)
                        {
                            return true;
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        public static int GetPlayerYLocationOnScreen()
        {
            Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen())
            {
                for (var y = (int)(curentResolution.Height * 0.25d); y < bitMap.Size.Height; y++)
                {
                    var pixel = bitMap.GetPixel(ConfigManager.GetPosFromConfig("TeamButtonXAndSize").X, y);

                    if (pixel.R >= 223 && pixel.R <= 243 && pixel.G >= 144 && pixel.G <= 164 && pixel.B >= 58 && pixel.B <= 78)
                    {
                        return y;
                    }
                }
            }

            return 0;
        }

        public static Tuple<int, int> GetArmyDropdownHeightAndMinYPos()
        {
            using (Bitmap bitMap = GrabScreen())
            {
                int height = -1;
                int yMin = 0;

                for (var y = GetPlayerYLocationOnScreen(); y < bitMap.Size.Height; y++)
                {
                    var pixel = bitMap.GetPixel(ConfigManager.GetPosFromConfig("TeamButtonXAndSize").X, y);

                    if (pixel.R == 122 && pixel.G == 48 && pixel.B == 1)
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
    }
}
