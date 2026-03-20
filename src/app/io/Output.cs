using ImageMagick;
using Wacton.Unicolour;

using App.Core;

namespace App.Io;

public static class Output
{
    public static void OutputImage(MagickImage image, Options opts)
    {
        if (opts.PrintImage)
        {
            image.Write(Console.OpenStandardOutput());
        }
        else
        {
            image.Write(opts.OutputFile);
        }
    }

    public static void Write(List<IMagickColor<byte>> palette, Options opts, Buckets buckets)
    {
        if (opts.Print)
        {
            if (!opts.HistogramOnly)
            {
                Console.WriteLine(Format.LineSeparator);
            }
            
            Console.WriteLine("Palette: ");
            foreach(IMagickColor<byte> color in palette)
            {
                Console.WriteLine($"Color: {color.ToHexString()}");
            }
        }
        else if (opts.RemapImage)
        {
            QuantizeSettings settings = new()
            {
                ColorSpace = ColorSpace.Lab,
                DitherMethod = DitherMethod.FloydSteinberg
            };

            using MagickImage remapImage = new(opts.RemapFile ?? opts.InputFile);
            remapImage.Remap(palette, settings);
            OutputImage(remapImage, opts);
        }
        else if (opts.AsGPL)
        {
            List<string> file = Format.AsGpl(palette, Path.GetFileNameWithoutExtension(opts.OutputFile));
            File.WriteAllLines(opts.OutputFile, file);
        }
        else if (opts.DisplayBins)
        {
            List<ColorHsv> colors2 = buckets.PaletteHsv().ToList();

            List<IMagickColor<byte>> palette2 = [];
            
            foreach (var color in colors2)
            {
                var b = new Unicolour(ColourSpace.Hsb, color.H * 360, color.S, color.V).Rgb.Byte255;
                palette2.Add(new MagickColor((byte)b.R, (byte)b.G, (byte)b.B));
            }

            using MagickImage bins = Format.AsPng2(palette2);
            bins.Write(Console.OpenStandardOutput());
        }
        else 
        {
            using MagickImage paletteImage = Format.AsPng(palette);
            OutputImage(paletteImage, opts);
        }
    }
}
