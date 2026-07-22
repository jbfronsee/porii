namespace App.Io;

public static class Help
{
    public const string InvalidArg = "Invalid Argument: {0}";

    public const string InvalidResize = "-r value must be between 0 and 100";
    
    public const string MissingMapOutput = "Please specify -o or -p argument for map subcommand";

    public const string MissingOutput = "Missing output file specified with -o [Filepath]";
    
    public const string Usage = 
        "Usage: porii InputImage [Options]" +
        "\n\nGenerates a palette from a JPG or PNG image using Magick.NET and Unicolour." +
        "\nIt can output to PNG for visualization and sampling or GPL format for importing into GIMP or Krita." +
        "\n\nOptions:" +
        "\n  -f <Strength> Sets the filter strength for the histogram based on number of pixels (low, medium, or high)" +
        "\n  -g            Outputs the palette as a GPL palette file" +
        "\n  -h            Only uses a histogram for generating the palette" +
        "\n  -o <File>     Outputs the palette as an image to a destination" +
        "\n  -p            Prints the palette as binary PNG image data to standard output" +
        "\n  -r <Percent>  Resizes the image by a percentage before generating the palette" +
        "\n  -v            Verbose printing" + 
        "\n\nCommands:" +
        "\n  map           Remaps palette onto an image using dithering" +
        "\n                Usage: porii map PaletteImage [RemapImage] [Options]\n";

    /// <summary>
    /// Returns Error Message for user from Options object if there are errors.
    /// </summary>
    /// <param name="opts">Options to check.</param>
    /// <returns>The error message or an empty string.</returns>
    public static string ErrorMessage(Options opts) => opts switch
    {
        { InputFile: null or [] } => Usage,
        { Print: false, PrintImage: false, OutputFile: null or [] } => MissingOutput,
        { InvalidArg: not (null or []) } => string.Format(InvalidArg, opts.InvalidArg),
        { ResizePercentage: > 100 or <= 0 } => InvalidResize,
        { RemapImage: true, PrintImage: false, OutputFile: null or [] } => MissingMapOutput,
        _ => ""
    };

    /// <summary>
    /// Prints errors present in Options if there are errors.
    /// </summary>
    /// <param name="opts">Options object we are checking for errors on.</param>
    /// <returns>true if errors were printed false if there are no errors on Options.</returns>
    public static bool PrintOptionErrors(Options opts)
    {
        string error = ErrorMessage(opts);
        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine(error);
            return true;
        }

        return false;
    }
}
