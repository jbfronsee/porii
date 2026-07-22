namespace App.Io;

public static class Help
{

    /// <summary>
    /// Returns Error Message for user from Options object if there are errors.
    /// </summary>
    /// <param name="opts">Options to check.</param>
    /// <returns>The error message or an empty string.</returns>
    public static string ErrorMessage(Options opts) => opts switch
    {
        { InputFile: null or [] } => "Usage: porii [InputFile] [Flags]",
        { Print: false, PrintImage: false, OutputFile: null or [] } => "Missing output file specified with -o [Filepath]",
        { InvalidArg: not (null or []) } => $"Invalid Argument: {opts.InvalidArg}",
        { ResizePercentage: > 100 or <= 0 } => $"-r value must be between 0 and 100",
        { RemapImage: true, PrintImage: false, OutputFile: null or [] } => $"Please specify -o or -p argument for map subcommand",
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
