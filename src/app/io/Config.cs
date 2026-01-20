using Microsoft.Extensions.Configuration;

using App.Core;

namespace App.Io;

public class Config
{
    private const string Buckets = "Buckets";

    private const string Saturated = "Saturated";

    private const string Desaturated = "Desaturated";

    private const string Hues = "Hues";

    private const string sampleDimensions = "SampleWhenAboveDimensions";

    private const string X = "X";

    private const string Y = "Y";

    public static Buckets GetBuckets(IConfigurationRoot config)
    {
        var bucketsConfig = config.GetSection("Buckets");
        Buckets buckets = new()
        {
            SaturatedHues = bucketsConfig.GetSection(Saturated).GetSection(Hues).Get<List<double>>() ?? [],
            DesaturatedHues = bucketsConfig.GetSection(Desaturated).GetSection(Hues).Get<List<double>>() ?? []
        };

        return buckets;
    }

    public static ((int, int), string) GetSampleDimensions(IConfigurationRoot config)
    {
        var sampleConfig = config.GetSection(sampleDimensions);

        int x = sampleConfig.GetSection(X).Get<int>();
        int y = sampleConfig.GetSection(Y).Get<int>();

        if (x <= 16 || y <= 16)
        {
            return ((0, 0), "Please specify values greater than 16 to sample above from.");
        }

        return ((x, y), "");
    }
}
