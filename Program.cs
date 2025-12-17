using System.Drawing;
using ImageMagick;
using ImageMagick.Colors;
using ImageMagick.Drawing;

internal class Program
{
    private static void Main(string[] args)
    {
        Options opts = Options.GetOptions(args);

        if (string.IsNullOrEmpty(opts.InputFile))
        {
            Console.WriteLine("Usage: PaletteGen [InputFile] [flags]");
            return;
        }
        else if (!string.IsNullOrEmpty(opts.InvalidArg))
        {
            Console.WriteLine($"Invalid Argument: {opts.InvalidArg}");
            return;
        }
        
        //Console.WriteLine("Processing Image...");

        var inputImage = new MagickImage(opts.InputFile);

        if (opts.PrintHistogram)
        {
            var histogram = inputImage.Histogram();
            foreach (var color in histogram)
            {
                Console.WriteLine($"Color: {color.Key} Occurences: {color.Value}");
            }

            return;
        }

        // if(inputImage.Width > 1080 || inputImage.Height > 1080)
        // {
        //     inputImage.Resize(new Percentage(75));
        // }

        List<IMagickColor<byte>> palette = Palette.PaletteFromImage(inputImage);

        palette = Palette.PaletteFromImageKmeans(inputImage, palette);
        //Console.WriteLine($"new hist {newHist.Count}");

        // Console.WriteLine("Palette: ");
        // foreach(string item in palette)
        // {
        //     Console.WriteLine($"Color: {item}");
        // }


        List<string> gplLines = new List<string>
        {
            "GIMP Palette",
            "Name: Test",
            $"Columns: {palette.Count}",
            "#"
        };
        int i = 0;
        foreach (IMagickColor<byte> color in palette)
        {
            gplLines.Add($"{color.R,3} {color.G,3} {color.B,3}\t#{i}");
            i++;
        }
        File.WriteAllLines(@"test.gpl", gplLines);
        
        MagickImage paletteImage = new MagickImage(MagickColors.Transparent, 512, 128);

        Drawables canvas = new Drawables();

        double x = 0, y = 0, width = 64, height = 64;
        foreach(IMagickColor<byte> color in palette)
        {
            // Define the rectangle's properties
            canvas
                .StrokeColor(color)
                .StrokeWidth(2)
                .FillColor(color)
                .Rectangle(x, y, x + width, y + height);

            x += width;
            if (x > 512)
            {
                x = 0;
                y += height;
            }
        }

        // Draw the square onto the image
        canvas.Draw(paletteImage);

        // Save the result

        paletteImage.Format = MagickFormat.Png;
        paletteImage.Write(Console.OpenStandardOutput());

        // paletteImage.Format = MagickFormat.Png;
        // paletteImage.Write(opts.OutputFile);
    }
}