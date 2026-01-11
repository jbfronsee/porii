using ImageMagick;
using ImageMagick.Colors;
using Wacton.Unicolour;

public class Palette
{
    /// <summary>
    /// Enables colors within a certain threshold to be considered a match in binary search and sorting functions.
    /// </summary>
    /// <param name="tolerances">The tolerances that decide the thresholds for matching.</param>
    private class ThresholdHsvComparer(Tolerances tolerances) : SimpleColor.HsvComparer
    {
        private Tolerances mTolerances = tolerances;

        public override int Compare(SimpleColor.Hsv x, SimpleColor.Hsv y)
        {
            int result = base.Compare(x, y);
            if (result != 0 && ColorWithinThreshold(x, y, mTolerances))
            {
                result = 0;
            }

            return result;
        }
    }

    private static double UpdateMean(double mean, double count, double newValue)
    {
        double sum = mean * count;
        return (sum + newValue) / (count + 1);
    }

    private static (SimpleColor.Lab, int) UpdateMeanColor((SimpleColor.Lab, int) mean, SimpleColor.Lab newColor)
    {
        var (color, count) = mean;
        return (
            new SimpleColor.Lab
            (
                UpdateMean(color.L, count, newColor.L),
                UpdateMean(color.A, count, newColor.A),
                UpdateMean(color.B, count, newColor.B)
            ),
            count + 1
        );
    }

    /// <summary>
    /// Calculates difference in Hue from normalized value to a Degree Value (0 - 360)
    /// </summary>
    /// 
    /// <param name="hue1">First hue to compare (0 - 1) normalized value</param>
    /// <param name="hue2">Second hue to compare (0 - 1) normalized value</param>
    public static double CalculateDeltaH(double hue1, double hue2)
    {
        // Hue is an angular slider that wraps around like a circle.
        double hDegrees1 = hue1 * 360;
        double hDegrees2 = hue2 * 360;
        double hueDiff = Math.Abs(hDegrees2 - hDegrees1);
        return Math.Min(360 - hueDiff, hueDiff);
    }

    /// <summary>
    /// Calculate distance between 2 colors in LAB space.
    /// </summary>
    /// <param name="color1">First Color.</param>
    /// <param name="color2">Second Color.</param>
    /// <returns>The distance between them.</returns>
    public static double CalculateDistance(SimpleColor.Lab color1, SimpleColor.Lab color2)
    {
        double deltaL = Math.Abs(color2.L - color1.L);
        double deltaA = Math.Abs(color2.A - color1.A);
        double deltaB = Math.Abs(color2.B - color1.B);
        return Math.Sqrt(deltaL * deltaL + deltaA * deltaA + deltaB * deltaB);
    }

    /// <summary>
    /// Creates Palette from image using Histogram
    /// </summary>
    /// 
    /// <param name="color1">First color to compare</param>
    /// <param name="color2">Second color to compare</param>
    public static bool ColorWithinThreshold(SimpleColor.Hsv color1, SimpleColor.Hsv color2, Tolerances tolerances)
    {
        double deltaH = CalculateDeltaH(color1.H, color2.H);
        double deltaS = Math.Abs(color2.S - color1.S);
        double deltaV = Math.Abs(color2.V - color1.V);

        ThresholdHsv thresh = tolerances.GetThreshold(color1.V);
        return deltaH <= thresh.Hue && deltaS <= thresh.Saturation && deltaV <= thresh.Value;
    }

    /// <summary>
    /// Creates Palette from pixels using Histogram
    /// </summary>
    /// 
    /// <param name="pixels">The pixels of the image in HSV space.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromPixels(ByteColor.Hsv[] pixels, Tolerances tolerances)
    {
        SortedDictionary<SimpleColor.Hsv, int> histogram = new(new ThresholdHsvComparer(tolerances));
        
        // Sample uniformly around image in steps.
        int step = (pixels.Length / 256) + 1;
        for (int i = 0; i < pixels.Length; i += step)
        {
            histogram.TryAdd(HsvConverter.ToSimple(pixels[i]), 1);
        }

        foreach(var color in pixels.Select(HsvConverter.ToSimple))
        {
            // If the color is within threshold update the max value.
            if (histogram.ContainsKey(color))
            {
                histogram[color]++;
            }
            else
            {
                histogram.Add(color, 1);
            }
        }

        var maxes = histogram.OrderByDescending(g => g.Value).Take(16).ToList();
        List<IMagickColor<byte>> palette = maxes.OrderBy(g => g.Key, new SimpleColor.HsvComparer()).Select(pair =>
        {
            var (color, _) = pair;
            return new ColorHSV(color.H, color.S, color.V).ToMagickColor();
        }).ToList();

        return palette;
    }

    /// <summary>
    /// Generates a palette using a Histogram.
    /// </summary>
    /// <param name="image">The image to generate from.</param>
    /// <param name="tolerances">The tolerances that represent threshold for histogram to find a match.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromImage(MagickImage image, Tolerances tolerances)
    {
        ByteColor.Hsv[] hsvPixels = image.GetPixelColors().Select(c =>
        {
            var hsv = ColorHSV.FromMagickColor(new MagickColor(c.R, c.G, c.B)) ?? new ColorHSV(0, 0, 0);
            return HsvConverter.ToByte(hsv.Hue, hsv.Saturation, hsv.Value);
        }).ToArray();

        return FromPixels(hsvPixels, tolerances);
    }

    /// <summary>
    /// Generates one K-Means Cluster from pixels.
    /// </summary>
    /// <param name="pixels">The pixels of the image in LAB space.</param>
    /// <param name="clusters">Previous cluster values.</param>
    /// <returns></returns>
    private static SimpleColor.Lab[] KMeansCluster(ByteColor.Lab[] pixels, SimpleColor.Lab[] clusters)
    {
        // Total colors in an image is usually a lot less than the number of pixels.
        Dictionary<ByteColor.Lab, int> memoizedCluster = [];
        
        (SimpleColor.Lab, int)[] means = clusters.Select(c => (c, 0)).ToArray();
        foreach (var color in pixels)
        {
            SimpleColor.Lab colorSimple = LabConverter.ToSimple(color);

            int bestClusterIndex = 0;
            if (!memoizedCluster.TryGetValue(color, out bestClusterIndex))
            {
                double bestDistance = double.MaxValue;
                for (int i = 0; i < clusters.Length; i++)
                {
                    double distance = CalculateDistance(clusters[i], colorSimple);
                    if (distance < bestDistance)
                    {
                        bestClusterIndex = i;
                        bestDistance = distance;
                    }
                }

                memoizedCluster[color] = bestClusterIndex;
            }
            
            SimpleColor.Lab bestCluster = clusters[bestClusterIndex];
            means[bestClusterIndex] = UpdateMeanColor(means[bestClusterIndex], colorSimple);
        }

        return means.Select(m =>
        {
            var (mean, _) = m;
            return mean;
        }).ToArray();
    }

    /// <summary>
    /// Generates a palette using K-Means Clustering from an array of LAB space pixels.
    /// </summary>
    /// <param name="pixels">The pixels of the image in LAB space.</param>
    /// <param name="seeds">The seed values to make initial clusters from.</param>
    /// <param name="verbose">Flag that enables printing K-Means progress message.</param>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromPixelsKmeans(ByteColor.Lab[] pixels, List<IMagickColor<byte>> seeds, bool verbose = false)
    {
        int maxIterations = 32;
        SimpleColor.Lab[] clusters = seeds.Select(c =>
        {
            var (l, a, b) = new Unicolour(ColourSpace.Rgb255, c.R, c.G, c.B).Lab;
            return new SimpleColor.Lab(l, a, b);
        }).ToArray();

        if (verbose)
        {
            Console.WriteLine($"K-Means Cluster Index: ");
        }

        bool finished = false;
        for (int i = 0; (i < maxIterations) && !finished; i++)
        {
            if (verbose)
            {
                Console.WriteLine(i);
            }

            SimpleColor.Lab[] newClusters = KMeansCluster(pixels, clusters);

            // If there is not a lot of change based on epsilon value then stop iterating.
            finished = newClusters.Zip(clusters).Aggregate(true, (total, pair) =>
            {
                var (cluster1, cluster2) = pair;
                return total && (CalculateDistance(cluster1, cluster2) <= 2.0);
            });

            clusters = newClusters;
        }

        var palette = clusters.Select(c =>
        {
            Unicolour color = new Unicolour(ColourSpace.Lab, c.L, c.A, c.B);
            var (h, s, v) = color.Hsb;
            return new SimpleColor.Hsv(h / 360, s, v);
        }).ToList();

        palette.Sort(new SimpleColor.HsvComparer());
        return palette.Select(c => new ColorHSV(c.H, c.S, c.V).ToMagickColor()).ToList();
    }

    /// <summary>
    /// Generates a palette using K-Means Clustering.
    /// </summary>
    /// <param name="image">The image to generate from.</param>
    /// <param name="seeds">The seed values to make initial clusters from.</param>
    /// <param name="verbose">Flag that enables printing K-Means progress message.</param>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromImageKmeans(MagickImage image, List<IMagickColor<byte>> seeds, bool verbose = false)
    {
        ByteColor.Lab[] labPixels = image.GetPixelColors().Select(c =>
        {
            var (l, a, b) = new Unicolour(ColourSpace.Rgb255, c.R, c.G, c.B).Lab;
            return LabConverter.ToByte(l, a, b);
        }).ToArray();
            
        return FromPixelsKmeans(labPixels, seeds, verbose);
    }
}