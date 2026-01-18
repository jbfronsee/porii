using ImageMagick;
using ImageMagick.Colors;
using Microsoft.Extensions.Configuration;
using Wacton.Unicolour;

using App.Core;
using App.Io;
using Lib.Analysis;
using Lib.Colors;

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

    public static void GeneratePalette(Options opts, IMagickImage<byte> image, Buckets buckets, IMagickImage<byte>? originalImage = null)
    {
        originalImage ??= image;

        HistogramLab histogram = Palette.CalculateHistogramFromSample(image, buckets);
        
        List<VectorLab> paletteLab = histogram
            .PaletteWithFilter(opts.FilterLevel)
            .DistinctBy(e => e.Mean)
            .OrderByDescending(e => e.Count)
            .Select(c => c.Mean)
            .Take(16)
            .ToList();

        if (paletteLab.Count < 16)
        {
            paletteLab = paletteLab.Concat(histogram.Palette().Where(p => !paletteLab.Contains(p.Mean)).Select(e => e.Mean)).Take(16).ToList();
        }

        List<IMagickColor<byte>> palette = paletteLab.Select(Colors.Convert.ToHsv)
            .OrderBy(c => c)
            .Select(c => new ColorHSV(c.H, c.S, c.V).ToMagickColor())
            .ToList();

        if (!opts.HistogramOnly)
        {
            palette = Palette.FromImageKmeans(image, palette, histogram.Colormap, opts.Verbose || opts.Print);

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

            List<ColorHsv> colors = buckets.PaletteHsv().ToList();
            
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
            if (opts.DisplayBins)
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
                // TODO this is debug temporary stuff :)
                return;
            }

            using MagickImage paletteImage = Format.AsPng(palette);

            if (opts.RemapImage)
            {
                var settings = new QuantizeSettings();
                settings.ColorSpace = ColorSpace.Lab;
                settings.DitherMethod = DitherMethod.FloydSteinberg;
                originalImage.Remap(palette, settings);
                originalImage.Write(Console.OpenStandardOutput());

                //TODO cleanup
                return;
            }

            if (opts.PrintImage)
            {
                paletteImage.Write(Console.OpenStandardOutput());
            }
            else
            {
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

            (Buckets buckets, string errorMessage) = ReadBuckets(config, opts.Verbose);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return;
            }
            
            using MagickImage inputImage = new(opts.InputFile);

            if (opts.Print || opts.Verbose)
            {
                Console.WriteLine("Processing Image...");
                Console.WriteLine(Format.LineSeparator);
            }

            inputImage.Settings.BackgroundColor = MagickColors.White;
            inputImage.Alpha(AlphaOption.Remove);

            if ( opts.ResizePercentage < 100 && opts.ResizePercentage > 0)
            {
                using IMagickImage<byte> sampled = inputImage.Clone();
                sampled.Sample(new Percentage(opts.ResizePercentage));
                GeneratePalette(opts, sampled, buckets, inputImage);
            }
            else
            {
                GeneratePalette(opts, inputImage, buckets);
            }
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