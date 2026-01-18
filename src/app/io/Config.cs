using Microsoft.Extensions.Configuration;

using App.Core;

namespace App.Io;

public class Config
{
    private const string Saturated = "Saturated";

    private const string Desaturated = "Desaturated";

    private const string Hues = "Hues";

    public static (Buckets, string) GetBuckets(IConfigurationSection config)
    {
        Buckets buckets = new();
        string errorMessage = "";
        buckets.SaturatedHues = config.GetSection(Saturated).GetSection(Hues).Get<List<double>>() ?? [];
        buckets.DesaturatedHues = config.GetSection(Desaturated).GetSection(Hues).Get<List<double>>() ?? [];

        return (buckets, errorMessage);
    }
}
