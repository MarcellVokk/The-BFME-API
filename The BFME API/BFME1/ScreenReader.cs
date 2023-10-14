﻿using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Runtime.InteropServices;

namespace The_BFME_API.BFME1
{
    internal static class ScreenReader
    {
        public static bool IsMenu1Visible()
        {
            System.Drawing.Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen().ToBitmap())
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
            System.Drawing.Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen().ToBitmap())
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
            System.Drawing.Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen().ToBitmap())
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
            System.Drawing.Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen().ToBitmap())
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
            System.Drawing.Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen().ToBitmap())
            {
                for (var y = (int)(curentResolution.Height * 0.25d); y < bitMap.Size.Height; y++)
                {
                    var pixel = bitMap.GetPixel(ConfigManager.GetPosFromConfig("TeamButtonXAndSize").X, y);

                    if (pixel.GetHue() > 16.5 && pixel.GetHue() < 22 && pixel.GetBrightness() * 100 > 25 && pixel.GetBrightness() * 100 < 40)
                    {
                        return y;
                    }
                }
            }

            return 0;
        }

        public static int GetArmyDropdownHeight()
        {
            System.Drawing.Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen().ToBitmap())
            {
                int height = -1;

                for (var y = GetPlayerYLocationOnScreen(); y < bitMap.Size.Height; y++)
                {
                    var pixel = bitMap.GetPixel(ConfigManager.GetPosFromConfig("TeamButtonXAndSize").X, y);

                    if (pixel.R == 122 && pixel.G == 48 && pixel.B == 1)
                    {
                        if (height != -1)
                        {
                            return height;
                        }
                        else
                        {
                            height = 0;
                        }
                    }
                    else if(height != -1)
                    {
                        height++;
                    }
                }
            }

            return 0;
        }

        public static List<Rectangle> GetMapSpots(System.Drawing.Point mapOffset)
        {
            List<Rectangle> spots = new List<Rectangle>();

            System.Drawing.Point ingameMapSize = ConfigManager.GetPosFromConfig("MapSize");
            System.Drawing.Point ingameMapTopLeft = ConfigManager.GetPosFromConfig("MapTopLeft");

            ingameMapTopLeft.Offset(mapOffset);

            System.Drawing.Size curentResolution = GameDataManager.GetCurentResolution();

            using (Bitmap bitMap = GrabScreen().ToBitmap())
            {
                for (int i = 0; i < 8; i++)
                {
                    System.Drawing.Point spotFirstPixelPos = System.Drawing.Point.Empty;

                    int minY = 0;
                    int maxY = 0;

                    int minX = 0;
                    int maxX = 0;

                    int padding = 0;

                    bool foundSpot = false;
                    for (var y = 0; y < bitMap.Height; y++)
                    {
                        for (var x = 0; x < bitMap.Width; x++)
                        {
                            if (isSpotPixel(x, y, bitMap))
                            {
                                spotFirstPixelPos = new System.Drawing.Point(x, y);
                                foundSpot = true;
                                break;
                            }
                        }
                        if (foundSpot) break;
                    }

                    if (!foundSpot) break;

                    for (var y = spotFirstPixelPos.Y; y < bitMap.Height; y++)
                    {
                        if (!isSpotPixel(spotFirstPixelPos.X, y, bitMap, true))
                        {
                            minY = y;
                            break;
                        }
                    }

                    for (var y = minY; y < bitMap.Height; y++)
                    {
                        if (isSpotPixel(spotFirstPixelPos.X, y, bitMap, true))
                        {
                            maxY = y;
                            break;
                        }
                    }

                    for (var x = spotFirstPixelPos.X; x > 0; x--)
                    {
                        if (isSpotPixel(x, minY + (maxY - minY) / 2, bitMap, true))
                        {
                            minX = x;
                            break;
                        }
                    }

                    for (var x = spotFirstPixelPos.X; x < bitMap.Width; x++)
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
            using (Bitmap bitMap = GrabScreen().ToBitmap())
            {
                int mistakes = 0;

                for(int i = 1; i < 14; i++)
                {
                    var pixelPosition = ConfigManager.GetPosFromConfig($"VictoryPixel{i}");
                    var pixel = bitMap.GetPixel(pixelPosition.X, pixelPosition.Y);

                    if ((pixel.R >= 237 && pixel.R <= 241) && (pixel.G >= 218 && pixel.G <= 228) && (pixel.B >= 155 && pixel.B <= 164))
                    {
                        continue;
                    }
                    else
                    {
                        pixel = bitMap.GetPixel(pixelPosition.X - (i >= 11 ? 4 : 0), pixelPosition.Y - 6);

                        if ((pixel.R >= 205 && pixel.R <= 220) && (pixel.G >= 220 && pixel.G <= 232) && (pixel.B >= 230 && pixel.B <= 245))
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

        public static Mat GrabScreen()
        {
            System.Drawing.Size curentResolution = GameDataManager.GetCurentResolution();

            Bitmap screenshot = new Bitmap(curentResolution.Width, curentResolution.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Graphics gfxScreenshot = Graphics.FromImage(screenshot);

            Rectangle screen = GetPrimaryScreenBounds();

            gfxScreenshot.CopyFromScreen(screen.X, screen.Y, 0, 0, curentResolution, CopyPixelOperation.SourceCopy);

            gfxScreenshot.Dispose();

            Mat refMat = BitmapConverter.ToMat(screenshot);
            screenshot.Dispose();

            Cv2.CvtColor(refMat, refMat, ColorConversionCodes.RGBA2RGB);

            return refMat;
        }
    }
}
