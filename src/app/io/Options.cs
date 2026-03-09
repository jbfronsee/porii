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

    private string mInvalidArg;

    public static Options GetOptions(string[] args)
    {
        Options opts = new();

        if (args.FirstOrDefault("") == "map")
        {
            opts.RemapImage = true;
        }

        opts.InputFile = args.Skip(1).FirstOrDefault("");

        // TODO temp solution
        bool isSecondFile = true;
        char? pairFlag = null;
        foreach (string arg in args.Skip(2))
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
            else if (isSecondFile)
            {
                opts.RemapFile = arg;
            }
            else
            {
                opts.mInvalidArg = arg;
                return opts;
            }

            isSecondFile = false;
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
