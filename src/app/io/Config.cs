using App.Core;
using Microsoft.Extensions.Configuration;

namespace App.Io;

public class Config
{
    public const string BucketPointId = "Point";

    public const string SaturationKey = "Saturation";

    public const string ValueKey = "Value";

    public const string BinsKey = "Bins";

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

    private static BucketPoint? ReadPoint(IConfigurationSection point)
    {
        BucketPoint? bucketPoint = null;
        if 
        (
            double.TryParse(point[SaturationKey], out double saturation) && 
            double.TryParse(point[ValueKey], out double value) &&
            int.TryParse(point[BinsKey], out int bins)
        )
        {
            return new BucketPoint(saturation, value, bins);
        }

        return bucketPoint;
    }

    public static int GetIndex(string pointKey)
    {
        bool isValid = pointKey.StartsWith(BucketPointId);
        
        if (isValid && int.TryParse(pointKey[BucketPointId.Length..], out int index) && index >= 1)
        {
            return index - 1;
        }

        return -1;
    }

    public static (Buckets, string) GetBuckets(IConfigurationSection config)
    {
        Buckets buckets = new();
        string errorMessage = "";
        buckets.SaturatedHues = config.GetSection("Saturated").GetSection("Hues").Get<List<double>>() ?? [];
        buckets.DesaturatedHues = config.GetSection("Desaturated").GetSection("Hues").Get<List<double>>() ?? [];


        // foreach (var my in my_doubles ?? [])
        // {
        //     Console.WriteLine(my);
        // }

        // foreach (var child in config.GetChildren().OrderBy(c => GetIndex(c.Key)))
        // {
        //     int index = GetIndex(child.Key);
            
        //     if (index < 0)
        //     {
        //         errorMessage = "Cannot retrieve index from Bucket Point.";
        //         break;
        //     }


        //     if (ReadPoint(child) is BucketPoint bucketPoint)
        //     {
        //         buckets.Points.Add(bucketPoint);
        //     }
        //     else
        //     {
        //         errorMessage = $"Point{index} is not a valid Bucket Point please specify Saturation and Value.";
        //     }
        // }

        return (buckets, errorMessage);
    }
}