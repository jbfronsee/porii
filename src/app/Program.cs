using App.Core;
using App.Io;
using ImageMagick;
using Microsoft.Extensions.Configuration;
using Wacton.Unicolour;

using SimpleColor = Lib.SimpleColor;
using Conversion = Lib.Conversion;

namespace App;

internal class Program
{
    public static void HandleException(Exception exception, string message, bool verbose)
    {
        if (verbose)
        {
            Console.WriteLine(exception);
        }
        else
        {
            Console.WriteLine(message);
        }
    }

    public static void GeneratePalette(Options opts, MagickImage inputImage, Tolerances tolerances, Buckets buckets)
    {
        if ( opts.ResizePercentage < 100 && opts.ResizePercentage > 0)
        {
            inputImage.Resize(new Percentage(opts.ResizePercentage));
        }

        inputImage.Settings.BackgroundColor = MagickColors.White;
        inputImage.Alpha(AlphaOption.Remove);

        List<IMagickColor<byte>> palette = Palette.FromImage(inputImage, tolerances, buckets);

        if (!opts.HistogramOnly)
        {
            palette = Palette.FromImageKmeans(inputImage, palette, opts.Verbose || opts.Print);

            if (opts.Print)
            {
                Console.WriteLine(Format.LineSeparator);
            }
        }

        if (opts.Print)
        {
            Console.WriteLine("Palette: ");
            foreach(IMagickColor<byte> color in palette)
            {
                Console.WriteLine($"Color: {color.ToHexString()}");
            }
        }
        else if (opts.AsGPL)
        {

            List<SimpleColor.Hsv> colors = buckets.PaletteHsv().ToList();
            
            List<IMagickColor<byte>> palette2 = [];
            
            foreach (var color in colors)
            {
                var b = new Unicolour(ColourSpace.Hsb, color.H * 360, color.S, color.V).Rgb.Byte255;
                palette2.Add(new MagickColor((byte)b.R, (byte)b.G, (byte)b.B));
            }
            
            List<string> file = Format.AsGpl(palette, Path.GetFileNameWithoutExtension(opts.OutputFile));
            File.WriteAllLines(opts.OutputFile, file);
        }
        else
        {
            List<SimpleColor.Hsv> colors2 = buckets.PaletteHsv().ToList();

            List<IMagickColor<byte>> palette2 = [];
            
            foreach (var color in colors2)
            {
                var b = new Unicolour(ColourSpace.Hsb, color.H * 360, color.S, color.V).Rgb.Byte255;
                palette2.Add(new MagickColor((byte)b.R, (byte)b.G, (byte)b.B));
            }

            using MagickImage paletteImage = Format.AsPng(palette);

            if (opts.PrintImage)
            {
                var settings = new QuantizeSettings();
                settings.ColorSpace = ColorSpace.Lab;
                settings.DitherMethod = DitherMethod.FloydSteinberg;
                inputImage.Remap(palette, settings);
                //inputImage.Write(Console.OpenStandardOutput());
                paletteImage.Write(Console.OpenStandardOutput());
            }
            else
            {
                var settings = new QuantizeSettings();
                settings.DitherMethod = DitherMethod.FloydSteinberg;
                settings.ColorSpace = ColorSpace.Lab;
                inputImage.Remap(palette, settings);
                //inputImage.Write(opts.OutputFile);
                paletteImage.Write(opts.OutputFile);
            }
        }
    }

    public static bool HasErrors(Options opts)
    {
        bool hasErrors = false;
        if (string.IsNullOrEmpty(opts.InputFile))
        {
            Console.WriteLine("Usage: px-swatch [InputFile] [Flags]");
            hasErrors = true;
        }
        else if (opts.Print == false && opts.PrintImage == false && string.IsNullOrEmpty(opts.OutputFile))
        {
            Console.WriteLine("Missing output file specified with -o [Filepath]");
            hasErrors = true;
        }
        else if (!string.IsNullOrEmpty(opts.InvalidArg))
        {
            Console.WriteLine($"Invalid Argument: {opts.InvalidArg}");
            hasErrors = true;
        }
        else if (opts.ResizePercentage > 100 || opts.ResizePercentage <= 0)
        {
            Console.WriteLine($"-r value must be between 0 and 100");
            hasErrors = true;
        }

        return hasErrors;
    }

    public static (Buckets, string) ReadBuckets(IConfigurationRoot config, bool verbose)
    {
        (Buckets buckets, string errorMessage) = Config.GetBuckets(config.GetSection("Buckets"));
        // if (!buckets.Points.Any())
        // {
        //     string error = "Invalid or missing appsettings.json file.";
        //     Console.WriteLine(error);
        //     return (buckets, error);
        // }

        // if (verbose)
        // {
        //     Console.WriteLine(Format.LineSeparator);
        //     Console.WriteLine($"Tolerances:\n{tolerances}");
        //     Console.WriteLine(Format.LineSeparator);
        // }

        // (bool bucketValid, string bucketMessage) = buckets.Validate();
        // if (!bucketValid)
        // {
        //     Console.WriteLine(bucketMessage);
        //     return (buckets, bucketMessage);
        // }

        return (buckets, "");
    }

    public static Tolerances? ReadTolerances(IConfigurationRoot config, bool verbose)
    {
        Tolerances? tolerances = Config.GetTolerances(config.GetSection("Tolerances"));
        if (tolerances is null)
        {
            Console.WriteLine("Invalid or missing appsettings.json file.");
            return tolerances;
        }

        if (verbose)
        {
            Console.WriteLine(Format.LineSeparator);
            Console.WriteLine($"Tolerances:\n{tolerances}");
            Console.WriteLine(Format.LineSeparator);
        }

        (bool tolValid, string tolMessage) = tolerances.Validate();
        if (!tolValid)
        {
            Console.WriteLine(tolMessage);
            return null;
        }

        return tolerances;
    }

    private static void Main(string[] args)
    {
        Options opts = Options.GetOptions(args);

        if (HasErrors(opts))
        {
            return;
        }

        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            Tolerances? tolerances = ReadTolerances(config, opts.Verbose);
            (Buckets buckets, string errorMessage) = ReadBuckets(config, opts.Verbose);
            if (tolerances is null || !string.IsNullOrEmpty(errorMessage))
            {
                return;
            }
            
            using MagickImage inputImage = new(opts.InputFile);

            if (opts.Print || opts.Verbose)
            {
                Console.WriteLine("Processing Image...");
                Console.WriteLine(Format.LineSeparator);
            }

            GeneratePalette(opts, inputImage, tolerances, buckets);
        }
        catch (MagickBlobErrorException mbee)
        {
            HandleException(mbee, $"Input file: {opts.InputFile} does not exist or is not an image.", opts.Verbose);
        }
        catch (MagickMissingDelegateErrorException mmdee)
        {
            HandleException(mmdee, $"Input file: {opts.InputFile} does not exist or is not an image.", opts.Verbose);
        }
        catch (Exception e)
        {
            HandleException(e, e.Message, opts.Verbose);
        }
    }
}