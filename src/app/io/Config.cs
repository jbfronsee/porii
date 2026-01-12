using Microsoft.Extensions.Configuration;

public class Config
{
    public static ThresholdHsv? ToThreshold(string? hueStr, string? saturationStr, string? valueStr, string? startStr)
    {
        if (
            double.TryParse(hueStr, out double hue) &&
            double.TryParse(saturationStr, out double saturation) &&
            double.TryParse(valueStr, out double value) &&
            double.TryParse(startStr, out double start)
        )
        {
            return new ThresholdHsv(hue, saturation, value, start);
        }

        return null;
    }

    private static ThresholdHsv? ReadThreshold(IConfigurationSection config, string valueRange)
    {
        var section = config.GetSection(valueRange);
        ThresholdHsv? threshold= ToThreshold(
            section["Hue"],
            section["Saturation"],
            section["Value"],
            section["Start"]
        );
        return threshold;
    }

    public static Tolerances? GetTolerances(IConfigurationSection config)
    {
        Tolerances? result = null;

        ThresholdHsv? darks = ReadThreshold(config, "Darks");
        ThresholdHsv? shadows = ReadThreshold(config, "Shadows");
        ThresholdHsv? midtones = ReadThreshold(config, "Midtones");
        ThresholdHsv? brights = ReadThreshold(config, "Brights");

        if (darks is not null && shadows is not null && midtones is not null && brights is not null)
        {
            result = new Tolerances(darks, shadows, midtones, brights);
        }

        return result;
    }
}