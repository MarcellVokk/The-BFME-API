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
                    var pixel = bitMap.GetPixel(ConfigManager.GetPosFromConfig("TeamButton1").X, y);

                    if (pixel.GetHue() > 16.5 && pixel.GetHue() < 22 && pixel.GetBrightness() * 100 < 40 && pixel.GetBrightness() * 100 > 25)
                    {
                        return y;
                    }
                }
            }

            return 0;
        }

        public static bool IsVictoriousTitleVisible()
        {
            using (Bitmap bitMap = GrabScreen().ToBitmap())
            {
                int mistakes = 0;

                for(int i = 1; i < 13; i++)
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
