using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using MatthiWare.CommandLine;

namespace cubemap_converter
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.Title = "cubemap_converter";

            // create the directories and files
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\bin"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\bin");
            }

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\bin\\images"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\bin\\images");
            }

            // parse command line arguments
            var parser = new CommandLineParser<Options>();
            parser.Configure(opt => opt.hcross).Name("hcross").Description("Path to horizontal cross cubemap.");
            parser.Configure(opt => opt.vcross).Name("vcross").Description("Path to vertical cross cubemap.");
            parser.Configure(opt => opt.vrow).Name("vrow").Description("Path to vertical row cubemap.");
            parser.Configure(opt => opt.hrow).Name("hrow").Description("Path to horizontal row cubemap.");
            var result = parser.Parse(args);

            // save all the faces into the //bin//images folder
            var encoder = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
            var encoderParams = new EncoderParameters() { Param = new[] { new EncoderParameter(Encoder.Quality, 100L) } };
            var filename = 0;

            Bitmap inputCubemap = null;
            if (result.HasErrors || args.Length <= 0)
            {
                foreach (var exception in result.Errors)
                    Console.WriteLine(exception.Message);

                Console.ReadKey();

                return;
            }
            else if (result.Result.hcross.Length > 0)
            {
                inputCubemap = new Bitmap(result.Result.hcross);

                foreach (var face in GraphicsFuncs.ReturnSeperatedHorizontalCross(inputCubemap, GraphicsFuncs.GreatestCommonFactor(inputCubemap.Height, inputCubemap.Width)))
                {
                    face.Save(Directory.GetCurrentDirectory() + $"//bin//images//{ filename }.jpg", encoder, encoderParams);
                    filename++;
                }
            }
            else if (result.Result.vcross.Length > 0)
            {
                inputCubemap = new Bitmap(result.Result.vcross);

                // vcross processing
            }
            else if (result.Result.hrow.Length > 0)
            {
                inputCubemap = new Bitmap(result.Result.hrow);

                // hrow processing
            }
            else if (result.Result.vrow.Length > 0)
            {
                inputCubemap = new Bitmap(result.Result.vrow);

                // vrow processing
            }
            Console.WriteLine("successfully seperated cubemap into seperate files");


            File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\bin\\texassemble.exe", Properties.Resources.texassemble);
            Console.WriteLine($"created { Directory.GetCurrentDirectory() }\\bin\\texassemble.exe");

            File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\bin\\texconv.exe", Properties.Resources.texconv);
            Console.WriteLine($"created { Directory.GetCurrentDirectory() }\\bin\\texconv.exe");

            // assemble sky.dds
            if (File.Exists(Directory.GetCurrentDirectory() + "\\bin\\texassemble.exe"))
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

        private class Options
        {
            public string hcross { get; set; }
            public string vcross { get; set; }
            public string vrow { get; set; }
            public string hrow { get; set; }
        }
    }
}
