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
                Bitmap inputCubemap = new Bitmap(args[0]);

                // create the directories and files
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

                // save all the faces into the //bin//images folder
                var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encoderParams = new EncoderParameters() { Param = new[] { new EncoderParameter(Encoder.Quality, 100L) } };
                var filename = 0;

                foreach (var image in GraphicsFuncs.ReturnSeperatedCubemap(inputCubemap, GraphicsFuncs.GreatestCommonFactor(inputCubemap.Height, inputCubemap.Width)))
                {
                    image.Save(Directory.GetCurrentDirectory() + $"//bin//images//{ filename }.jpg", encoder, encoderParams);
                    filename++;
                }

                Console.WriteLine("successfully seperated cubemap into seperate files");

                // assemble sky.dds
                if(File.Exists(Directory.GetCurrentDirectory() + "\\bin\\texassemble.exe"))
                {
                    Console.WriteLine("assembling dds...");
                    string assembleArgs = $"cube { Directory.GetCurrentDirectory() }\\bin\\images\\0.jpg { Directory.GetCurrentDirectory() }\\bin\\images\\1.jpg { Directory.GetCurrentDirectory() }\\bin\\images\\2.jpg" +
                        $" { Directory.GetCurrentDirectory() }\\bin\\images\\3.jpg { Directory.GetCurrentDirectory() }\\bin\\images\\4.jpg { Directory.GetCurrentDirectory() }\\bin\\images\\5.jpg -o { Directory.GetCurrentDirectory() }" +
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
