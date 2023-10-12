using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace The_BFME_API_by_MarcellVokk.BFME1
{
    public static class ComputerVisionManager
    {
        public static bool GetGameStartedVisibility()
        {
            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
            string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

            using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\game_started.png", ImreadModes.Unchanged))
            using (Mat refMat = GrabScreen())
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Mat refGray = new Mat();
                Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                Mat tplGray = tplMat;
                Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        return true;
                    }
                    else
                        break;
                }
            }

            return false;
        }

        public static bool IsStartFailedTextMessageVisible()
        {
            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
            string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

            using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\start_failed_test_message.png", ImreadModes.Unchanged))
            using (Mat refMat = GrabScreen())
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Mat refGray = new Mat();
                Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                Mat tplGray = tplMat;
                Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        return true;
                    }
                    else
                        break;
                }
            }

            return false;
        }

        public static bool IsMenu1Visible()
        {
            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
            string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

            using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\menu_1_multiplayer_button.png", ImreadModes.Unchanged))
            using (Mat refMat = GrabScreen())
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Mat refGray = new Mat();
                Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                Mat tplGray = tplMat;
                Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        return true;
                    }
                    else
                        break;
                }
            }

            return false;
        }

        public static bool IsMenu2Visible()
        {
            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
            string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

            using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\menu_2_network_button.png", ImreadModes.Unchanged))
            using (Mat refMat = GrabScreen())
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Mat refGray = new Mat();
                Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                Mat tplGray = tplMat;
                Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        return true;
                    }
                    else
                        break;
                }
            }

            return false;
        }

        public static bool IsMenuCustomGameLobbyVisible()
        {
            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
            string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

            using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\custom_game_lobby_title.png", ImreadModes.Unchanged))
            using (Mat refMat = GrabScreen())
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Mat refGray = new Mat();
                Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                Mat tplGray = tplMat;
                Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        return true;
                    }
                    else
                        break;
                }
            }

            return false;
        }

        public static bool IsInLobby(bool isHost)
        {
            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
            string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

            if (isHost)
            {
                using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\host_game_title.png", ImreadModes.Unchanged))
                using (Mat refMat = GrabScreen())
                using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
                {
                    Mat refGray = new Mat();
                    Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                    Mat tplGray = tplMat;
                    Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                    Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                    Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                    while (true)
                    {
                        double minval, maxval, threshold = 0.8;
                        OpenCvSharp.Point minloc, maxloc;
                        Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                        if (maxval >= threshold)
                        {
                            return true;
                        }
                        else
                            break;
                    }
                }
            }
            else
            {
                using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\join_game_title.png", ImreadModes.Unchanged))
                using (Mat refMat = GrabScreen())
                using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
                {
                    Mat refGray = new Mat();
                    Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                    Mat tplGray = tplMat;
                    Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                    Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                    Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                    while (true)
                    {
                        double minval, maxval, threshold = 0.8;
                        OpenCvSharp.Point minloc, maxloc;
                        Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                        if (maxval >= threshold)
                        {
                            return true;
                        }
                        else
                            break;
                    }
                }
            }

            return false;
        }

        public static bool IsJoinFailedPopupVisible()
        {
            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
            string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

            using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\join_failed_popup_title.png", ImreadModes.Unchanged))
            using (Mat refMat = GrabScreen())
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Mat refGray = new Mat();
                Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                Mat tplGray = tplMat;
                Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        return true;
                    }
                    else
                        break;
                }
            }

            return false;
        }

        public static int GetPlayerYLocationOnScreen()
        {
            ConfigManager.ReloadConfig();

            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
            string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

            using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\button_down.png", ImreadModes.Unchanged))
            using (Mat refMat = GrabScreen())
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Mat refMatRange = refMat.SubMat(0, refMat.Rows, ConfigManager.GetIntFromConfig("ArmyButtonArrayStartX"), ConfigManager.GetIntFromConfig("ArmyButtonArrayEndX"));

                Mat refGray = new Mat();
                Cv2.CvtColor(refMatRange, refGray, ColorConversionCodes.RGB2GRAY);

                Mat tplGray = tplMat;
                Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        return maxloc.Y;
                    }
                    else
                        break;
                }

                return 0;
            }
        }

        public static bool IsVictoriousTitleVisible()
        {
            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();
            string resolutionId = $"{curentResolution.Width}x{curentResolution.Height}";

            using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\victorious.png", ImreadModes.Unchanged))
            using (Mat refMat = GrabScreen())
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Mat refGray = new Mat();
                Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                Mat tplGray = tplMat;
                Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        return true;
                    }
                    else
                        break;
                }
            }

            using (Mat tplMat = new Mat($@".\BFME1\bfme_api_resources\{resolutionId}\victorious_red.png", ImreadModes.Unchanged))
            using (Mat refMat = GrabScreen())
            using (Mat res = new Mat(refMat.Rows - tplMat.Rows + 1, refMat.Cols - tplMat.Cols + 1, MatType.CV_32FC1))
            {
                Mat refGray = new Mat();
                Cv2.CvtColor(refMat, refGray, ColorConversionCodes.RGB2GRAY);

                Mat tplGray = tplMat;
                Cv2.CvtColor(tplMat, tplGray, ColorConversionCodes.RGBA2GRAY);

                Cv2.MatchTemplate(refGray, tplGray, res, TemplateMatchModes.CCoeffNormed);
                Cv2.Threshold(res, res, 0.8, 1.0, ThresholdTypes.Tozero);

                while (true)
                {
                    double minval, maxval, threshold = 0.8;
                    OpenCvSharp.Point minloc, maxloc;
                    Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);

                    if (maxval >= threshold)
                    {
                        return true;
                    }
                    else
                        break;
                }
            }

            return false;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

        public static Mat GrabScreen()
        {
            System.Drawing.Size curentResolution = ResolutionManager.GetResolutionFromSettings();

            Bitmap screenshot = new Bitmap(curentResolution.Width, curentResolution.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Graphics gfxScreenshot = Graphics.FromImage(screenshot);

            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, curentResolution, CopyPixelOperation.SourceCopy);

            gfxScreenshot.Dispose();

            Mat refMat = BitmapConverter.ToMat(screenshot);
            screenshot.Dispose();

            Cv2.CvtColor(refMat, refMat, ColorConversionCodes.RGBA2RGB);

            return refMat;
        }
    }
}
