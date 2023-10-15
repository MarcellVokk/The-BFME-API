using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                System.Drawing.Point spotFirstPixelPos = System.Drawing.Point.Empty;

                int minY = 0;
                int maxY = 0;

                int minX = 0;
                int maxX = 0;

                int padding = 0;

                bool foundSpot = false;
                for (var y = 0; y < mapImage.Height; y++)
                {
                    for (var x = 0; x < mapImage.Width; x++)
                    {
                        if (isSpotPixel(x, y, mapImage))
                        {
                            spotFirstPixelPos = new System.Drawing.Point(x, y);
                            foundSpot = true;
                            break;
                        }
                    }
                    if (foundSpot) break;
                }

                if (!foundSpot) break;

                for (var y = spotFirstPixelPos.Y; y < mapImage.Height; y++)
                {
                    if (!isSpotPixel(spotFirstPixelPos.X, y, mapImage, true))
                    {
                        minY = y;
                        break;
                    }
                }

                for (var y = minY; y < mapImage.Height; y++)
                {
                    if (isSpotPixel(spotFirstPixelPos.X, y, mapImage, true))
                    {
                        maxY = y;
                        break;
                    }
                }

                for (var x = spotFirstPixelPos.X; x > 0; x--)
                {
                    if (isSpotPixel(x, minY + (maxY - minY) / 2, mapImage, true))
                    {
                        minX = x;
                        break;
                    }
                }

                for (var x = spotFirstPixelPos.X; x < mapImage.Width; x++)
                {
                    if (isSpotPixel(x, minY + (maxY - minY) / 2, mapImage, true))
                    {
                        maxX = x;
                        break;
                    }
                }

                padding = minY - spotFirstPixelPos.Y + 2;

                var spot = new Rectangle(minX, minY, maxX - minX, maxY - minY);

                spots.Add(new Rectangle(spot.X - padding - 1, spot.Y - padding, spot.Width + 2 * padding + 4, spot.Height + 2 * padding + 2));
            }

            using (Mat m = mapImage.ToMat())
            {
                Cv2.CvtColor(m, m, ColorConversionCodes.RGB2BGR);

                int index = 0;
                foreach (var item in spots)
                {
                    m.Rectangle(new Rect(item.X, item.Y, item.Width, item.Height), new Scalar(255, 0, 0), 1);
                    m.PutText(index.ToString(), new OpenCvSharp.Point(item.X - 5, item.Y + 5), HersheyFonts.HersheySimplex, 1, new Scalar(255, 255, 255));
                    m.Circle(new OpenCvSharp.Point(item.X + item.Width / 2, item.Y + item.Height / 2), 3, new Scalar(0, 255, 0), -1);
                    index++;
                }

                Cv2.CvtColor(m, m, ColorConversionCodes.BGR2RGB);

                return m.ToBitmap();
            }

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
    }
}
