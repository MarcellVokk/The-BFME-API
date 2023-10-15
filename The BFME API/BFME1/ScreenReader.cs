﻿using System.Drawing;
using System.Runtime.InteropServices;

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

        public static List<Rectangle> GetMapSpots(Point mapOffset)
        {
            List<Rectangle> spots = new List<Rectangle>();

            Point ingameMapSize = ConfigManager.GetPosFromConfig("MapSize");
            Point ingameMapTopLeft = ConfigManager.GetPosFromConfig("MapTopLeft");

            ingameMapTopLeft.Offset(mapOffset);

            Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen())
            {
                for (int i = 0; i < 8; i++)
                {
                    Point spotFirstPixelPos = System.Drawing.Point.Empty;

                    int minY = 0;
                    int maxY = 0;

                    int minX = 0;
                    int maxX = 0;

                    int padding = 0;

                    bool foundSpot = false;
                    for (var y = ingameMapTopLeft.Y; y < ingameMapTopLeft.Y + ingameMapSize.Y; y++)
                    {
                        for (var x = ingameMapTopLeft.X; x < ingameMapTopLeft.X + ingameMapSize.X; x++)
                        {
                            if (isSpotPixel(x, y, bitMap))
                            {
                                spotFirstPixelPos = new Point(x, y);
                                foundSpot = true;
                                break;
                            }
                        }
                        if (foundSpot) break;
                    }

                    if (!foundSpot) break;

                    for (var y = spotFirstPixelPos.Y; y < ingameMapTopLeft.Y + ingameMapSize.Y; y++)
                    {
                        if (!isSpotPixel(spotFirstPixelPos.X, y, bitMap, true))
                        {
                            minY = y;
                            break;
                        }
                    }

                    for (var y = minY; y < ingameMapTopLeft.Y + ingameMapSize.Y; y++)
                    {
                        if (isSpotPixel(spotFirstPixelPos.X, y, bitMap, true))
                        {
                            maxY = y;
                            break;
                        }
                    }

                    for (var x = spotFirstPixelPos.X; x > ingameMapTopLeft.X; x--)
                    {
                        if (isSpotPixel(x, minY + (maxY - minY) / 2, bitMap, true))
                        {
                            minX = x;
                            break;
                        }
                    }

                    for (var x = spotFirstPixelPos.X; x < ingameMapTopLeft.X + ingameMapSize.X; x++)
                    {
                        if (isSpotPixel(x, minY + (maxY - minY) / 2, bitMap, true))
                        {
                            maxX = x;
                            break;
                        }
                    }

                    padding = minY - spotFirstPixelPos.Y + 2;

                    var spot = new Rectangle(minX, minY, maxX - minX, maxY - minY);

                    spots.Add(new Rectangle(spot.X - padding - 1, spot.Y - padding, spot.Width + 2 * padding + 4, spot.Height + 2 * padding + 2));
                }
            }

            return spots;

            bool isSpotPixel(int x, int y, Bitmap bitMap, bool allowKnownSpots = false)
            {
                if (!allowKnownSpots && spots.Any(e => e.Contains(x, y)))
                {
                    return false;
                }

                var pixel = bitMap.GetPixel(x, y);
                return pixel.GetHue() > 40 && pixel.GetHue() < 47 && pixel.GetSaturation() * 100 > 90 && pixel.GetBrightness() * 100 > 10;
            }
        }

        public static bool IsVictoriousTitleVisible()
        {
            using (Bitmap bitMap = GrabScreen())
            {
                int mistakes = 0;

                for(int i = 1; i < 14; i++)
                {
                    var pixelPosition = ConfigManager.GetPosFromConfig($"VictoryPixel{i}");
                    var pixel = bitMap.GetPixel(pixelPosition.X, pixelPosition.Y);

                    if (pixel.R >= 237 && pixel.R <= 241 && pixel.G >= 218 && pixel.G <= 228 && pixel.B >= 155 && pixel.B <= 164)
                    {
                        continue;
                    }
                    else
                    {
                        pixel = bitMap.GetPixel(pixelPosition.X - (i >= 11 ? 4 : 0), pixelPosition.Y - 6);

                        if (pixel.R >= 205 && pixel.R <= 220 && pixel.G >= 220 && pixel.G <= 232 && pixel.B >= 230 && pixel.B <= 245)
                        {
                            continue;
                        }
                        else
                        {
                            mistakes++;
                        }
                    }
                }

                return mistakes <= 3;
            }
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
