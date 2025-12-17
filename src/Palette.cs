using ImageMagick;
using ImageMagick.Colors;

class Palette
{
    /// <summary>
    /// Creates Palette from image using Histogram
    /// </summary>
    /// 
    /// <param name="image">The image to create palette from.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public static List<IMagickColor<byte>> PaletteFromImage(MagickImage image)
    {
        var hist = image.Histogram();

        Dictionary<ColorHSV, uint> maxValues = new Dictionary<ColorHSV, uint>();
        foreach (var color in hist)
        {
            ColorHSV colorHSV = ColorHSV.FromMagickColor(color.Key) ?? new ColorHSV(0, 0, 0);
            
            ColorHSV? index = null;
            uint min = uint.MaxValue;
            ColorHSV? indexReplace = null;
            foreach (var max in maxValues)

            {
                ColorHSV hColor = max.Key;
                double deltaH = Math.Abs(hColor.Hue - colorHSV.Hue);
                double deltaS = Math.Abs(hColor.Saturation - colorHSV.Saturation);
                double deltaV = Math.Abs(hColor.Value - colorHSV.Value);

                // TODO constants
                //TODO hue is angular
                double thresh_h = .02;
                double thresh_s = .3;
                double thresh_v = .3;

                if(colorHSV.Value < .2)
                {
                    thresh_s = 1;
                    thresh_h = 1;
                }
                if(colorHSV.Value > .2 && colorHSV.Value < .4)
                {
                    thresh_s = .8;
                    thresh_h *= 2;
                }
                if(colorHSV.Value > .4 && colorHSV.Value < .6)
                {
                    thresh_s *= 2;
                }

                if (deltaV > thresh_v || deltaH > thresh_h || deltaS > thresh_s)
                {
                    if(color.Value > min || color.Value > max.Value)
                    {
                        if(max.Value < min)
                        {
                            indexReplace = max.Key;
                            // Console.WriteLine($"hColor.Hue {hColor.Hue}");
                            // Console.WriteLine($"hColor.Saturation {hColor.Saturation}");
                            // Console.WriteLine($"hColor.Value {hColor.Value}");

                            // Console.WriteLine($"deltaH: {deltaH}");
                            // Console.WriteLine($"deltaS: {deltaS}");
                            // Console.WriteLine($"deltaV: {deltaV}");
                        }
                    }
                }
                else
                {
                    index = hColor;
                    break;
                }

                min = Math.Min(max.Value, min);
            }

            if (index != null)
            {
                maxValues[index] += color.Value;
            }
            else if (maxValues.Count < 16)
            {
                maxValues[colorHSV] = color.Value;
            }
            else if (indexReplace != null)
            {
                maxValues.Remove(indexReplace);
                maxValues[colorHSV] = color.Value;
            }
            
        }

        List<IMagickColor<byte>> palette = new List<IMagickColor<byte>>();
        foreach (var color in maxValues.OrderByDescending(c => c.Value))
        {
            palette.Add(color.Key.ToMagickColor());
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