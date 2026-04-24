using ImageMagick;
using ImageMagick.Colors;
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

    public static void GeneratePalette(Options opts, IMagickImage<byte> image, double largePixelCount, Buckets buckets)
    {
        IHistogramLab histogram = Palette.CalculateHistogramFromSample(image, buckets);
        
        //TODO zero colors
        List<IMagickColor<byte>> palette = [];
        if (histogram.Colormap.Count <= 256)
        {
            // TODO Lab vs RGB
            palette = [.. histogram.Results
                .Zip(histogram.Colormap.Keys)
                .OrderByDescending(z => z.First.Count)
                .Take(16)
                .Select(z => Colors.Convert.ToHsv(z.Second))
                .OrderBy(c => c)
                .Select(c => new ColorHSV(c.H, c.S, c.V).ToMagickColor())
            ];
        }
        else
        {
            palette = [.. histogram.FilteredPalette(opts.FilterLevel)
                .Select(Colors.Convert.ToHsv)
                .OrderBy(c => c)
                .Select(c => new ColorHSV(c.H, c.S, c.V).ToMagickColor())
            ];
        }

        if (!opts.HistogramOnly)
        {
            palette = Palette.FromImage(image, palette, largePixelCount, histogram.Colormap, opts.Verbose || opts.Print);
        }

        Output.Write(palette, opts, buckets);
    }

    public static bool HasErrors(Options opts)
    {
        bool hasErrors = false;
        if (string.IsNullOrEmpty(opts.InputFile))
        {
            Console.WriteLine("Usage: porii [InputFile] [Flags]");
            hasErrors = true;
        }
        else if (!opts.Print && !opts.PrintImage && string.IsNullOrEmpty(opts.OutputFile))
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
        else if (opts.RemapImage && !opts.PrintImage && string.IsNullOrEmpty(opts.OutputFile))
        {
            Console.WriteLine($"Please specify -o or -p argument for map subcommand.");
            hasErrors = true;
        }

        return hasErrors;
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
            ((int sampleX, int sampleY), errorMessage) = Config.GetSampleDimensions(config);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return;
            }

            double largePixelCount = sampleX * sampleY;
            
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
                GeneratePalette(opts, sampled, largePixelCount, buckets);
            }
            else
            {
                GeneratePalette(opts, inputImage, largePixelCount, buckets);
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