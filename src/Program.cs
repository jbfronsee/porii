using ImageMagick;
using Microsoft.Extensions.Configuration;

internal class Program
{
    public static void GeneratePalette(Options opts, MagickImage inputImage, Tolerances tolerances)
    {
        if (opts.ResizePercentage < 100 && opts.ResizePercentage > 0)
        {
            inputImage.Resize(new Percentage(opts.ResizePercentage));
        }

        inputImage.Settings.BackgroundColor = MagickColors.White;
        inputImage.Alpha(AlphaOption.Remove);

        List<IMagickColor<byte>> palette = Palette.FromImage(inputImage, tolerances);

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
            List<string> file = Format.AsGpl(palette, Path.GetFileNameWithoutExtension(opts.OutputFile));
            File.WriteAllLines(opts.OutputFile, file);
        }
        else
        {
            MagickImage paletteImage = Format.AsPng(palette);

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
            if (tolerances is null)
            {
                return;
            }
            
            MagickImage inputImage = new();
            
            try
            {
                inputImage = new MagickImage(opts.InputFile);
            }
            catch (MagickException)
            {
                Console.WriteLine($"Input file: {opts.InputFile} does not exist or is not an image.");
                return;
            }

            if (opts.Print || opts.Verbose)
            {
                Console.WriteLine("Processing Image...");
                Console.WriteLine(Format.LineSeparator);
            }

            GeneratePalette(opts, inputImage, tolerances);
        } 
        catch (Exception e)
        {
            if (opts.Verbose)
            {
                Console.WriteLine(e);
            }
            else
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}