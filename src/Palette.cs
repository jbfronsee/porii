using ImageMagick;
using ImageMagick.Colors;

public class Palette
{
    private class ColorHSVComparer(Tolerances tolerances) : IComparer<ColorHSV>
    {
        private Tolerances mTolerances = tolerances;

        public int Compare(ColorHSV? x, ColorHSV? y)
        {
            if (x is null)
            {
                return y is not null ? -y.CompareTo(x) : 0;
            }

            if (y is not null)
            {
                if (ColorWithinThreshold(x, y, mTolerances))
                {
                    return 0;
                }
            }

            return x.CompareTo(y);
        }
    }

    /// <summary>
    /// Creates Palette from image using Histogram
    /// </summary>
    /// 
    /// <param name="color1">First color to compare</param>
    /// <param name="color2">Second color to compare</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public static bool ColorWithinThreshold(ColorHSV color1, ColorHSV color2, Tolerances tolerances)
    {
        // Hue is an angular slider that wraps around like a circle.
        double hDegrees1 = color1.Hue * 360;
        double hDegrees2 = color2.Hue * 360;
        double hueDiff = Math.Abs(hDegrees2 - hDegrees1);
        
        // Value differences
        double deltaH = Math.Min(360 - hueDiff, hueDiff);
        double deltaS = Math.Abs(color2.Saturation - color1.Saturation);
        double deltaV = Math.Abs(color2.Value - color1.Value);

        ThresholdHSV thresh = tolerances.GetThreshold(color1.Value);
        return deltaH <= thresh.Hue && deltaS <= thresh.Saturation && deltaV <= thresh.Value;
    }

    /// <summary>
    /// Creates Palette from image using Histogram
    /// </summary>
    /// 
    /// <param name="image">The image to create palette from.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public static List<IMagickColor<byte>> PaletteFromImage(MagickImage image, Tolerances tolerances)
    {
        SortedDictionary<ColorHSV, uint> histogram = new(new ColorHSVComparer(tolerances));
        foreach (var color in image.GetPixels().Select(p => p.ToColor() ?? new MagickColor()))
        {
            ColorHSV hsv = ColorHSV.FromMagickColor(color) ?? new ColorHSV(0, 0, 0);

            // If the color is within threshold update the max value.
            if (histogram.ContainsKey(hsv))
            {
                histogram[hsv]++;
            }
            else
            {
                histogram.Add(hsv, 1);
            }
        }

        var maxes = histogram.OrderByDescending(g => g.Value).Take(16).ToList();
        List<IMagickColor<byte>> palette = [];
        foreach (var (color, _) in maxes)
        {
            palette.Add(color.ToMagickColor());
        }

        return palette;
    }

    /// <summary>
    /// Creates Palette from image using Kmeans. Modifies image object.
    /// </summary>
    /// 
    /// <param name="image">The image to create palette from.</param>
    /// <param name="seeds">The seed values for the Kmeans clusters.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public static List<IMagickColor<byte>> PaletteFromImageKmeans(MagickImage image, List<IMagickColor<byte>> seeds)
    {
        KmeansSettings kmeans = new KmeansSettings();
        kmeans.SeedColors = string.Join(";", seeds);
        kmeans.NumberColors = (uint)seeds.Count;
        kmeans.MaxIterations = 16;

        image.Kmeans(kmeans);

        var newHist = image.Histogram();

        List<IMagickColor<byte>> palette = new List<IMagickColor<byte>>();
        foreach (var color in newHist.OrderByDescending(c => c.Value))
        {
            palette.Add(color.Key);
        }

        return palette;
    }
}