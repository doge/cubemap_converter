using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace cubemap_converter
{
    public class GraphicsFuncs
    {
        public static Bitmap ReturnCroppedBitmap(Bitmap originalImage, int[] firstPoint, int[] secondPoint)
        {
            // calculate the width and height of our new image
            int width = Math.Abs(firstPoint[0] - secondPoint[0]);
            int height = Math.Abs(firstPoint[1] - secondPoint[1]);

            Bitmap area = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(area))
            {
                Rectangle destinationRect = new Rectangle(0, 0, width, height);
                Rectangle originalRect = new Rectangle(Math.Min(firstPoint[0], secondPoint[0]), Math.Min(firstPoint[1], secondPoint[1]), width, height);
                graphics.DrawImage(originalImage, destinationRect, originalRect, GraphicsUnit.Pixel);
            }

            return area;
        }

        public static int GreatestCommonFactor(int a, int b)
        {
            // calculate greatest common factor and return the result

            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a == 0 ? b : a;
        }

        public static Bitmap[] ReturnSeperatedHorizontalCross(Bitmap cubemap, int multiple)
        {
            Bitmap positiveX = ReturnCroppedBitmap(cubemap, new int[] { multiple * 2, multiple }, new int[] { multiple * 3, multiple * 2 });
            Bitmap negativeX = ReturnCroppedBitmap(cubemap, new int[] { 0, multiple }, new int[] { multiple, multiple * 2 });

            Bitmap positiveY = ReturnCroppedBitmap(cubemap, new int[] { multiple, 0 }, new int[] { multiple * 2, multiple });
            Bitmap negativeY = ReturnCroppedBitmap(cubemap, new int[] { multiple, multiple * 2 }, new int[] { multiple * 2, multiple * 3 });

            Bitmap positiveZ = ReturnCroppedBitmap(cubemap, new int[] { multiple, multiple }, new int[] { multiple * 2, multiple * 2 });
            Bitmap negativeZ = ReturnCroppedBitmap(cubemap, new int[] { multiple * 3, multiple }, new int[] { multiple * 4, multiple * 2 });

            return new Bitmap[] { positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ };
        }

        // https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
