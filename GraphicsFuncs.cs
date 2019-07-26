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
        public static Bitmap[] ReturnSeperatedHorizontalCross(Bitmap cubemap, int faceSize)
        {
            Bitmap positiveX = ReturnCroppedBitmap(cubemap, new int[] { faceSize * 2, faceSize }, new int[] { faceSize * 3, faceSize * 2 });
            Bitmap negativeX = ReturnCroppedBitmap(cubemap, new int[] { 0, faceSize }, new int[] { faceSize, faceSize * 2 });

            Bitmap positiveY = ReturnCroppedBitmap(cubemap, new int[] { faceSize, 0 }, new int[] { faceSize * 2, faceSize });
            Bitmap negativeY = ReturnCroppedBitmap(cubemap, new int[] { faceSize, faceSize * 2 }, new int[] { faceSize * 2, faceSize * 3 });

            Bitmap positiveZ = ReturnCroppedBitmap(cubemap, new int[] { faceSize, faceSize }, new int[] { faceSize * 2, faceSize * 2 });
            Bitmap negativeZ = ReturnCroppedBitmap(cubemap, new int[] { faceSize * 3, faceSize }, new int[] { faceSize * 4, faceSize * 2 });

            return new Bitmap[] { positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ };
        }

        public static Bitmap[] ReturnSeperatedVerticalCross(Bitmap cubemap, int faceSize)
        {
            // todo

            Bitmap positiveX = ReturnCroppedBitmap(cubemap, new int[] { faceSize * 2, faceSize }, new int[] { faceSize * 3, faceSize * 2 });
            Bitmap negativeX = ReturnCroppedBitmap(cubemap, new int[] { 0, faceSize }, new int[] { faceSize, faceSize * 2 });

            Bitmap positiveY = ReturnCroppedBitmap(cubemap, new int[] { faceSize, 0 }, new int[] { faceSize * 2, faceSize });


            Bitmap negativeY = ReturnCroppedBitmap(cubemap, new int[] { faceSize, faceSize * 2 }, new int[] { faceSize * 2, faceSize * 3 });

            Bitmap positiveZ = ReturnCroppedBitmap(cubemap, new int[] { faceSize, faceSize * 3 }, new int[] { faceSize * 2, faceSize * 4 });
            Bitmap negativeZ = ReturnCroppedBitmap(cubemap, new int[] { faceSize, faceSize }, new int[] { faceSize * 2, faceSize * 2 });

            return new Bitmap[] { positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ };
        }

        public static Bitmap[] ReturnSeperatedHorizontalRow(Bitmap cubemap, int faceSize)
        {
            // todo

            Bitmap positiveX = null;
            Bitmap negativeX = null;

            Bitmap positiveY = null;
            Bitmap negativeY = null;

            Bitmap positiveZ = null;
            Bitmap negativeZ = null;

            return new Bitmap[] { positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ };
        }

        public static Bitmap[] ReturnSeperatedVericalRow(Bitmap cubemap, int faceSize)
        {
            // todo

            Bitmap positiveX = null;
            Bitmap negativeX = null;

            Bitmap positiveY = null;
            Bitmap negativeY = null;

            Bitmap positiveZ = null;
            Bitmap negativeZ = null;

            return new Bitmap[] { positiveX, negativeX, positiveY, negativeY, positiveZ, negativeZ };
        }

        private static Bitmap ReturnCroppedBitmap(Bitmap originalImage, int[] firstPoint, int[] secondPoint)
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
