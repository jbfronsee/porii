class Options
{
    public static Options GetOptions(string[] args)
    {
        Options opts = new Options();
        bool output = false;
        foreach (string arg in args)
        {
            if (arg.StartsWith("-"))
            {
                if (arg.Length == 2)
                {
                    char flagChar = arg[1];
                    switch (flagChar)
                    {
                        case 'h':
                            opts.PrintHistogram = true;
                            break;
                        case 'i':
                            opts.PrintImage = true;
                            break;
                        case 'o':
                            output = true;
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
        InputFile = "";
        OutputFile = "";
        PrintHistogram = false;
        PrintImage = false;
        mInvalidArg = "";
    }

    public string InputFile { get; set; }

    public string OutputFile { get; set; }

    public string InvalidArg => mInvalidArg;

    public bool PrintHistogram { get; set; }

    public bool PrintImage { get; set; }
}