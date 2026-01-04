public class Options
{
    public static Options GetOptions(string[] args)
    {
        Options opts = new Options();
        bool output = false;
        bool resize = false;
        foreach (string arg in args)
        {
            if (arg.StartsWith("-"))
            {
                if (arg.Length == 2)
                {
                    char flagChar = arg[1];
                    switch (flagChar)
                    {
                        case 'g':
                            opts.AsGPL = true;
                            opts.Print = false;
                            break;
                        case 'h':
                            opts.HistogramOnly = true;
                            break;
                        case 'i':
                            opts.PrintImage = true;
                            opts.Print = false;
                            break;
                        case 'o':
                            output = true;
                            opts.Print = false;
                            break;
                        case 'r':
                            resize = true;
                            break;
                        case 'v':
                            opts.Verbose = true;
                            break;

                    }
                }
            }
            else
            {
                // Process non-flag arguments (operands/values)
                if (output)
                {
                    opts.OutputFile = arg;
                    output = false;
                }
                else if (resize)
                {
                    opts.ResizePercentage = double.Parse(arg);
                    resize = false;
                }
                else if (string.IsNullOrEmpty(opts.InputFile))
                {
                    opts.InputFile = arg;
                }
                else
                {
                    opts.mInvalidArg = arg;
                }
            }
        }

        return opts;
    }

    private string mInvalidArg;

    public Options()
    {
        mInvalidArg = "";

        AsGPL = false;
        InputFile = "";
        OutputFile = "";
        HistogramOnly = false;
        Print = true;
        PrintImage = false;
        ResizePercentage = 100;
        Verbose = false;
    }

    public bool AsGPL { get; set; }

    public string InputFile { get; set; }

    public string OutputFile { get; set; }

    public bool HistogramOnly { get; set; }

    public string InvalidArg => mInvalidArg;

    public bool Print { get; set; }

    public bool PrintImage { get; set; }

    public double ResizePercentage { get; set; }
    
    public bool Verbose { get; set; }
}