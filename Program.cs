using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;

namespace cubemap_converter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                Console.WriteLine("Drag the cubemap you wish to convert onto this exe.");
                Console.ReadKey();
            }
            else if(!File.Exists(args[0]))
            {
                Console.WriteLine("The path to the file is invalid.");
                Console.ReadKey();
            }
            else
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\bin"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\bin");
                }

                if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\bin\\images"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\bin\\images");
                }

                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\bin\\texassemble.exe", Properties.Resources.texassemble);
                Console.WriteLine($"created { Directory.GetCurrentDirectory() }\\bin\\texassemble.exe");

                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\bin\\texconv.exe", Properties.Resources.texconv);
                Console.WriteLine($"created { Directory.GetCurrentDirectory() }\\bin\\texconv.exe");


                GraphicsFuncs gFunc = new GraphicsFuncs();
                Bitmap cubemap = new Bitmap(args[0]);

                if (cubemap.Height != 1536 || cubemap.Width != 2048)
                {
                    cubemap = gFunc.ResizeImage(cubemap, 2048, 1536);
                    Console.WriteLine("converted image to 2048x1536");
                }

                Bitmap positiveX = gFunc.ReturnCroppedBitmap(cubemap, new int[] { 1024, 512 }, new int[] { 1536, 1024 });
                Bitmap negativeX = gFunc.ReturnCroppedBitmap(cubemap, new int[] { 0, 512 }, new int[] { 512, 1024 });

                Bitmap positiveY = gFunc.ReturnCroppedBitmap(cubemap, new int[] { 512, 0 }, new int[] { 1024, 512 });
                Bitmap negativeY = gFunc.ReturnCroppedBitmap(cubemap, new int[] { 512, 1024 }, new int[] { 1024, 1536 });

                Bitmap positiveZ = gFunc.ReturnCroppedBitmap(cubemap, new int[] { 512, 512 }, new int[] { 1024, 1024 });
                Bitmap negativeZ = gFunc.ReturnCroppedBitmap(cubemap, new int[] { 1536, 512 }, new int[] { 2048, 1024 });

                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encParams = new EncoderParameters() { Param = new[] { new EncoderParameter(Encoder.Quality, 100L) } };

                positiveX.Save(Directory.GetCurrentDirectory() + "\\bin\\images\\positiveX.jpg", encoder, encParams);
                negativeX.Save(Directory.GetCurrentDirectory() + "\\bin\\images\\negativeX.jpg", encoder, encParams);

                positiveY.Save(Directory.GetCurrentDirectory() + "\\bin\\images\\positiveY.jpg", encoder, encParams);
                negativeY.Save(Directory.GetCurrentDirectory() + "\\bin\\images\\negativeY.jpg", encoder, encParams);

                positiveZ.Save(Directory.GetCurrentDirectory() + "\\bin\\images\\positiveZ.jpg", encoder, encParams);
                negativeZ.Save(Directory.GetCurrentDirectory() + "\\bin\\images\\negativeZ.jpg", encoder, encParams);

                /*

                Bitmap converted = new Bitmap(3072, 512);
                using (Graphics g = Graphics.FromImage(converted))
                {
                    g.DrawImage(positiveX, 0, 0);
                    g.DrawImage(negativeX, 512, 0);

                    g.DrawImage(positiveY, 1024, 0);
                    g.DrawImage(negativeY, 1536, 0);

                    g.DrawImage(positiveZ, 2048, 0);
                    g.DrawImage(negativeZ, 2560, 0);
                }
                converted.Save(Directory.GetCurrentDirectory() + "\\bin\\converted.jpg");

                */

                Console.WriteLine("successfully seperated cubemap into seperate files");

                // assemble sky.dds
                if(File.Exists(Directory.GetCurrentDirectory() + "\\bin\\texassemble.exe"))
                {
                    Console.WriteLine("assembling dds...");
                    string assembleArgs = $"cube { Directory.GetCurrentDirectory() }\\bin\\images\\positiveX.jpg { Directory.GetCurrentDirectory() }\\bin\\images\\negativeX.jpg { Directory.GetCurrentDirectory() }\\bin\\images\\positiveY.jpg" +
                        $" { Directory.GetCurrentDirectory() }\\bin\\images\\negativeY.jpg { Directory.GetCurrentDirectory() }\\bin\\images\\positiveZ.jpg { Directory.GetCurrentDirectory() }\\bin\\images\\negativeZ.jpg -o { Directory.GetCurrentDirectory() }" +
                        $"\\bin\\sky.dds -y";

                    var texassemble = Process.Start(Directory.GetCurrentDirectory() + "\\bin\\texassemble.exe", assembleArgs);
                    texassemble.StartInfo.CreateNoWindow = true;
                    texassemble.WaitForExit();
                    Console.WriteLine("assembled sky.dds");

                }

                // compress sky.dds
                if(File.Exists(Directory.GetCurrentDirectory() + "\\bin\\texconv.exe"))
                {
                    Console.WriteLine("compressing sky.dds...");
                    var texconv = Process.Start(Directory.GetCurrentDirectory() + "\\bin\\texconv.exe", $"{ Directory.GetCurrentDirectory() }\\bin\\sky.dds -o bin\\ -bcquick -f DXT1 -wiclossless -y");
                    texconv.StartInfo.CreateNoWindow = true;
                    texconv.WaitForExit();
                    Console.WriteLine("sky.dds compressed with dxt1");
                }

                // delete the images when we are done with them
                Directory.Delete(Directory.GetCurrentDirectory() + "\\bin\\images", true);

                // delete the directx binaries
                File.Delete(Directory.GetCurrentDirectory() + "\\bin\\texconv.exe");
                File.Delete(Directory.GetCurrentDirectory() + "\\bin\\texassemble.exe");

                Console.ReadKey();
            }
        }
    }
}
