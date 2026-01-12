public class Options
{
    private static bool IsFlag(string arg)
    {
        return arg.StartsWith("-") && arg.Length == 2;
    }

    private static bool IsPairFlag(char flag)
    {
        char[] flags = ['o', 'r'];
        return flags.Contains(flag);
    }

    private void HandleFlag(char flag)
    {
        switch (flag)
        {
            case 'g':
                AsGPL = true;
                Print = false;
                break;
            case 'h':
                HistogramOnly = true;
                break;
            case 'o':
                Print = false;
                break;
            case 'p':
                PrintImage = true;
                Print = false;
                break;
            case 'v':
                Verbose = true;
                break;
        }
    }

    private void HandlePairFlag(char flag, string arg)
    {
        if (IsFlag(arg) || string.IsNullOrEmpty(arg))
        {
            mInvalidArg = $"Missing operand for {flag}";
            return;
        }

        switch (flag)
        {
            case 'o':
                OutputFile = arg;
                break;
            case 'r':
                if (double.TryParse(arg, out double percentage))
                {
                    ResizePercentage = percentage;
                }
                else
                {
                    mInvalidArg = $"{arg} is not a percentage.";
                    return;
                }
                break;
        }
    }

    private string mInvalidArg;

    public static Options GetOptions(string[] args)
    {
        Options opts = new()
        {
            InputFile = args.FirstOrDefault("")
        };

        char? pairFlag = null;
        foreach (string arg in args.Skip(1))
        {
            if (pairFlag is not null)
            {
                opts.HandlePairFlag(pairFlag.Value, arg);
                pairFlag = null;
            }
            else if (IsFlag(arg))
            {
                char flag = arg[1];
                opts.HandleFlag(flag);
                if (IsPairFlag(flag))
                {
                    pairFlag = flag;
                }
            }
            else
            {
                opts.mInvalidArg = arg;
                return opts;
            }
        }

        if (pairFlag is not null)
        {
            opts.HandlePairFlag(pairFlag.Value, "");
        }

        return opts;
    }

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