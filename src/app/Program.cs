using ImageMagick;
using Microsoft.Extensions.Configuration;

using App.Core;
using App.Io;
using Lib.Analysis.Interfaces;

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

    public static List<IMagickColor<byte>> GetHistogramPalette(Options opts, IHistogramLab histogram)
    {
        return histogram.Colormap.Count switch
        {
            <= 16 => Colors.MagickSorting.SortByHsv(histogram.Colormap.Keys),
            <= 256 => Colors.MagickSorting.SortByHsv(histogram.Results
                .Zip(histogram.Colormap.Keys)
                .OrderByDescending(z => z.First.Count)
                .Take(16)
                .Select(z => z.Second)),
            _ => Colors.MagickSorting.SortByHsv(histogram.FilteredPalette(opts.FilterLevel))
        };
    }

    public static List<IMagickColor<byte>> GeneratePalette(Options opts, IMagickImage<byte> image, double largePixelCount, Buckets buckets)
    {
        IHistogramLab histogram = Palette.CalculateHistogramFromSample(image, buckets);
        List<IMagickColor<byte>> palette = GetHistogramPalette(opts, histogram);

        bool histOnly = opts.HistogramOnly || histogram.Colormap.Count <= 16;
        if (!histOnly)
        {
            palette = Palette.FromImage(image, palette, largePixelCount, histogram.Colormap, opts.Verbose || opts.Print);
        }

        return palette;
    }

    public static (Buckets, string) ReadBuckets(IConfigurationRoot config, bool verbose)
    {
        Buckets buckets = Config.GetBuckets(config);

        (bool bucketValid, string bucketMessage) = buckets.Validate();
        if (!bucketValid)
        {
            Console.WriteLine(bucketMessage);
            return (buckets, bucketMessage);
        }

        return (buckets, "");
    }

    public static IMagickImage<byte> ReadImage(Options opts)
    {
        MagickImage inputImage = new(opts.InputFile);

        if (opts.Print || opts.Verbose)
        {
            Console.WriteLine("Processing Image...");
            Console.WriteLine(Format.LineSeparator);
        }

        inputImage.Settings.BackgroundColor = MagickColors.White;
        inputImage.Alpha(AlphaOption.Remove);

        if ( opts.ResizePercentage < 100 && opts.ResizePercentage > 0)
        {
            inputImage.Sample(new Percentage(opts.ResizePercentage));
        }
        
        return inputImage;
    }

    public static List<IMagickColor<byte>> ReadPaletteFromGpl(Options opts)
    {
        List<IMagickColor<byte>> palette = [];
        
        try 
        {
            palette = Format.FromGpl(File.ReadLines(opts.InputFile));
        }
        catch (ArgumentOutOfRangeException aoore)
        {
            HandleException(aoore, "The gpl file does not have full rgb specified.", opts.Verbose);
        }
        catch (FormatException fe)
        {
            HandleException(fe, "The gpl file has incorrect formatting of color values.", opts.Verbose);
        }
        catch (OverflowException oe)
        {
            HandleException(oe, "The gpl file color values should be between 0 and 255.", opts.Verbose);
        }

        return palette;
    }

    public static List<IMagickColor<byte>> ReadPaletteFromImage(Options opts, double largePixelCount, Buckets buckets)
    {
        using IMagickImage<byte> inputImage = ReadImage(opts);
        return GeneratePalette(opts, inputImage, largePixelCount, buckets);
    }

    public static void WritePalette(Options opts, List<IMagickColor<byte>> palette, Buckets buckets)
    {
        if (palette.Count == 0)
        {
            Console.WriteLine($"No colors detected in {opts.InputFile}");
        }
        else
        {
            Output.Write(palette, opts, buckets);
        }
    }

    private static void Main(string[] args)
    {
        Options opts = Options.GetOptions(args);

        if (Help.PrintOptionErrors(opts))
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
            ((int sampleX, int sampleY), errorMessage) = Config.GetSampleDimensions(config);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return;
            }

            double largePixelCount = sampleX * sampleY;
            string extension = Path.GetExtension(opts.InputFile);
            List<IMagickColor<byte>> palette = extension switch
            {
                ".gpl" => ReadPaletteFromGpl(opts),
                _ => ReadPaletteFromImage(opts, largePixelCount, buckets)
            };

            WritePalette(opts, palette, buckets);       
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
