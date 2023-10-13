using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_BFME_API.BFME1;

namespace The_BFME_API_by_MarcellVokk.Tools
{
    public static class MapSpotPreviewTool
    {
        public static Bitmap DrawMapSpotsPreview(Bitmap mapImage)
        {
            List<Rectangle> spots = new List<Rectangle>();

            for (int i = 0; i < 8; i++)
            {
                int minx = -1;
                int maxx = -1;
                int miny = -1;
                int maxy = -1;

                for (var y = 0; y < mapImage.Height; y++)
                {
                    bool anySpotPixelInLine = false;

                    for (var x = 0; x < mapImage.Width; x++)
                    {
                        if (isSpotPixel(x, y, mapImage))
                        {
                            if (miny == -1)
                            {
                                miny = y;

                                minx = x;
                                maxx = x;
                            }

                            if (x < minx)
                            {
                                if (minx - x > 10)
                                {
                                    miny = y;
                                    maxx = x;
                                }

                                minx = x;
                            }

                            if (x > maxx)
                            {
                                if (x - maxx > 10)
                                {
                                    break;
                                }

                                maxx = x;
                            }

                            anySpotPixelInLine = true;
                            break;
                        }
                    }

                    if (miny != -1 && !anySpotPixelInLine)
                    {
                        maxy = y;
                        break;
                    }
                }

                if (minx != -1)
                {
                    for (var x = minx; x < mapImage.Width; x++)
                    {
                        bool anySpotPixelInLine = false;

                        for (var y = miny; y < maxy; y++)
                        {
                            if (isSpotPixel(x, y, mapImage))
                            {
                                if (minx == -1)
                                {
                                    minx = x;
                                }

                                anySpotPixelInLine = true;
                            }
                        }

                        if (minx != -1 && !anySpotPixelInLine)
                        {
                            maxx = x;
                            break;
                        }
                    }

                    spots.Add(new Rectangle(minx, miny, maxx - minx, maxy - miny));
                }
                else
                {
                    break;
                }
            }

            using (Mat m = mapImage.ToMat())
            {
                Cv2.CvtColor(m, m, ColorConversionCodes.RGB2BGR);

                int index = 0;
                foreach (var item in spots)
                {
                    m.Rectangle(new Rect(item.X, item.Y, item.Width, item.Height), new Scalar(255, 0, 0), 1);
                    m.PutText(index.ToString(), new OpenCvSharp.Point(item.X - 5, item.Y + 5), HersheyFonts.HersheySimplex, 1, new Scalar(255, 255, 255));
                    index++;
                }

                Cv2.CvtColor(m, m, ColorConversionCodes.BGR2RGB);

                return m.ToBitmap();
            }

            bool isSpotPixel(int x, int y, Bitmap bitMap)
            {
                if (spots.Any(e => e.Contains(x, y)))
                {
                    return false;
                }

                var pixel = bitMap.GetPixel(x, y);
                return pixel.GetHue() > 40 && pixel.GetHue() < 47 && pixel.GetSaturation() * 100 > 90 && pixel.GetBrightness() * 100 > 10;
            }
        }
    }
}
