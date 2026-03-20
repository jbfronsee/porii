using Lib.Analysis;

namespace App.Io;

public class Options
{
    private static bool IsFlag(string arg)
    {
        return arg.StartsWith("-") && arg.Length == 2;
    }

    private static bool IsPairFlag(char flag)
    {
        char[] flags = ['f', 'o', 'r'];
        return flags.Contains(flag);
    }

    public static Options GetOptions(string[] args)
    {
        Options opts = new();
        string[] remaining = [.. opts.HandleSubCommand(args), ""];

        for (int i = 0; (i + 1) < remaining.Length; i++)
        {
            string arg = remaining[i];
            string nextArg = remaining[i + 1];

            if (IsFlag(arg))
            {
                char flag = arg[1];
                opts.HandleFlag(flag);
                if (IsPairFlag(flag))
                {
                    opts.HandlePairFlag(flag, nextArg);
                    i++;
                }
            }
            else
            {
                opts.mInvalidArg = arg;
                return opts;
            }
        }

        return opts;
    }

    private void HandleFlag(char flag)
    {
        switch (flag)
        {
            case 'd':
                DisplayBins = true;
                break;
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
            default:
                mInvalidArg = $"-{flag} is not a flag.";
                break;
        }
    }

    private void HandlePairFlag(char flag, string arg)
    {
        if (IsFlag(arg) || string.IsNullOrEmpty(arg))
        {
            mInvalidArg = $"Missing operand for {flag}.";
            return;
        }

        switch (flag)
        {
            case 'f':
                if (Enum.TryParse(arg, true, out FilterStrength filterStrength))
                {
                    FilterLevel = filterStrength;
                }
                else
                {
                    mInvalidArg = $"{arg} is not a filter strength.";
                    return;
                }
                break;
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

    private IEnumerable<string> HandleSubCommand(string[] args)
    {
        switch (args.FirstOrDefault(""))
        {
            case "map":
                RemapImage = true;
                InputFile = args.Skip(1).FirstOrDefault("");
                RemapFile = args.Skip(2).FirstOrDefault("");
                return args.Skip(3);
            default:
                InputFile = args.FirstOrDefault("");
                return args.Skip(1);
        }
    }

    private string mInvalidArg;

    public Options()
    {
        mInvalidArg = "";

        AsGPL = false;
        DisplayBins = false;
        FilterLevel = FilterStrength.Medium;
        InputFile = "";
        OutputFile = "";
        HistogramOnly = false;
        RemapFile = "";
        RemapImage = false;
        Print = true;
        PrintImage = false;
        ResizePercentage = 100;
        Verbose = false;
    }

    public bool AsGPL { get; set; }

    public bool DisplayBins { get; set; }

    public FilterStrength FilterLevel { get; set; }

    public string InputFile { get; set; }

    public string OutputFile { get; set; }

    public bool HistogramOnly { get; set; }

    public string InvalidArg => mInvalidArg;

    public string RemapFile { get; set; }

    public bool RemapImage { get; set; }

    public bool Print { get; set; }

    public bool PrintImage { get; set; }

    public double ResizePercentage { get; set; }
    
    public bool Verbose { get; set; }
}
