using System.Drawing;

namespace The_BFME_API.BFME_Shared
{
    public static class SpotDetectionEngine
    {
        public static List<Rectangle> GetMapSpots(Bitmap mapImage, Rectangle? mapBounds = null)
        {
            Rectangle _mapBounds = new Rectangle(0, 0, mapImage.Width, mapImage.Height);

            if (mapBounds is not null) _mapBounds = mapBounds.Value;

            List<Rectangle> spots = new List<Rectangle>();

            for (int i = 0; i < 8; i++)
            {
                Point spotFirstPixelPos = Point.Empty;

                int minY = 0;
                int maxY = 0;

                int minX = 0;
                int maxX = 0;

                int padding = 0;

                bool foundSpot = false;
                for (var y = _mapBounds.Y; y < _mapBounds.Y + _mapBounds.Height; y++)
                {
                    for (var x = _mapBounds.X; x < _mapBounds.X + _mapBounds.Width; x++)
                    {
                        if (isSpotPixel(x, y, mapImage))
                        {
                            spotFirstPixelPos = new Point(x, y);
                            foundSpot = true;
                            break;
                        }
                    }
                    if (foundSpot) break;
                }

                if (!foundSpot) break;

                for (var y = spotFirstPixelPos.Y; y < _mapBounds.Y + _mapBounds.Height; y++)
                {
                    if (!isSpotPixel(spotFirstPixelPos.X, y, mapImage, true))
                    {
                        minY = y;
                        break;
                    }
                }

                for (var y = minY; y < _mapBounds.Y + _mapBounds.Height; y++)
                {
                    if (isSpotPixel(spotFirstPixelPos.X, y, mapImage, true))
                    {
                        maxY = y;
                        break;
                    }
                }

                for (var x = spotFirstPixelPos.X; x > _mapBounds.X; x--)
                {
                    if (isSpotPixel(x, minY + (maxY - minY) / 2, mapImage, true))
                    {
                        minX = x;
                        break;
                    }
                }

                for (var x = spotFirstPixelPos.X; x < _mapBounds.X + _mapBounds.Width; x++)
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
    }
}
