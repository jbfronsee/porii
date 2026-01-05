using Microsoft.Extensions.Configuration;

public class Config
{

    public static ThresholdHSV? ToThreshold(string? hueStr, string? saturationStr, string? valueStr)
    {
        if (
            double.TryParse(hueStr, out double hue) &&
            double.TryParse(saturationStr, out double saturation) &&
            double.TryParse(valueStr, out double value)
        )
        {
            return new ThresholdHSV(hue, saturation, value);
        }

        return null;
    }

    public static ThresholdHSV? ToThreshold(string[] hsv)
    {
        if (hsv.Length != 3)
        {
            return null;
        }

        return ToThreshold(hsv[0], hsv[1], hsv[2]);
    }

    private static ThresholdHSV? ReadThreshold(IConfigurationSection config, string valueRange)
    {
        var section = config.GetSection(valueRange);
        ThresholdHSV? threshold= ToThreshold(section["Hue"], section["Saturation"], section["Value"]);
        return threshold;
    }

    public static Tolerances? GetTolerances(IConfigurationSection config)
    {
        Tolerances? result = null;

        ThresholdHSV? darks = ReadThreshold(config, "Darks");
        ThresholdHSV? shadows = ReadThreshold(config, "Shadows");
        ThresholdHSV? midtones = ReadThreshold(config, "Midtones");
        ThresholdHSV? brights = ReadThreshold(config, "Brights");

        if (darks is not null && shadows is not null && midtones is not null && brights is not null)
        {
            result = new Tolerances()
            {
                Darks = darks,
                Shadows = shadows,
                Midtones = midtones,
                Brights = brights
            };
        }

        return result;
    }
}