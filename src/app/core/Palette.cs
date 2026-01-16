using Conversion = Lib.Conversion;
using SimpleColor = Lib.SimpleColor;

using App.Extensions;
using ImageMagick;
using ImageMagick.Colors;
using Wacton.Unicolour;
using System.Collections.Concurrent;
using Lib.Analysis;

namespace App.Core;

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


    /// <summary>
    /// Enables colors within a certain threshold to be considered a match in binary search and sorting functions.
    /// </summary>
    /// <param name="tolerances">The tolerances that decide the thresholds for matching.</param>
    private class ThresholdLabComparer() : SimpleColor.LabComparer
    {
        public override int Compare(SimpleColor.Lab x, SimpleColor.Lab y)
        {
            int result = base.Compare(x, y);
            if (result != 0 && CalculateDistanceSquared(x, y) < 100)
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

    private static double UpdateMean(double mean, double count, double otherMean, double otherCount)
    {
        double sum1 = mean * count;
        double sum2 = otherMean * otherCount;
        return (sum1 + sum2) / (count + otherCount);
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

    private static (SimpleColor.Lab, int) UpdateMeanColor((SimpleColor.Lab, int) mean, (SimpleColor.Lab, int) mean2)
    {
        var (color, count) = mean;
        var (color2, count2) = mean2;
        if (count == 0 && count2 == 0)
        {
            return mean;
        }

        return (
            new SimpleColor.Lab
            (
                UpdateMean(color.L, count, color2.L, count2),
                UpdateMean(color.A, count, color2.A, count2),
                UpdateMean(color.B, count, color2.B, count2)
            ),
            count + count2
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
    /// Calculate distance between 2 colors in LAB space.
    /// </summary>
    /// <param name="color1">First Color.</param>
    /// <param name="color2">Second Color.</param>
    /// <returns>The distance between them.</returns>
    public static double CalculateDistanceSquared(SimpleColor.Lab color1, SimpleColor.Lab color2)
    {
        double deltaL = Math.Abs(color2.L - color1.L);
        double deltaA = Math.Abs(color2.A - color1.A);
        double deltaB = Math.Abs(color2.B - color1.B);
        return deltaL * deltaL + deltaA * deltaA + deltaB * deltaB;
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
    
    private static double CalculateDistanceSquaredHsv(SimpleColor.Hsv color1, SimpleColor.Hsv color2)
    {
        double deltaH = CalculateDeltaH(color1.H, color2.H) / 360;
        double deltaS = Math.Abs(color2.S - color1.S);
        double deltaV = Math.Abs(color2.V - color1.V);

        return deltaH * deltaH + deltaS * deltaS + deltaV * deltaV;
    }

    /// <summary>
    /// Creates Palette from pixels using Histogram
    /// </summary>
    /// 
    /// <param name="pixels">The pixels of the image in HSV space.</param>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromPixels(SimpleColor.Rgb[] pixels, Dictionary<SimpleColor.Rgb, SimpleColor.PackedLab> colormap, Tolerances tolerances, Buckets buckets)
    {
        Histogram histogram = new();

        int j = 0;
        foreach (var bucket in buckets.PaletteRgb())
        {
            // TODO Duplicate values are present
            var (l, a, b) = new Unicolour(ColourSpace.Rgb255, bucket.R, bucket.G, bucket.B).Lab;
            SimpleColor.Lab labBucket = new(l, a, b);
            Histogram.Entry entry = new()
            {
                Bucket = Conversion.Lab.Pack(labBucket),
                Average = Conversion.Lab.Pack(labBucket),
                Count = 0
            };
            histogram.Results[j] = entry;
            j++;
        }

        Dictionary<SimpleColor.Rgb, int> memoizedBucket = [];

        foreach(var pixel in pixels)
        {
            SimpleColor.Lab color = Conversion.Lab.Unpack(colormap[pixel]);

            int bestBucketIndex = 0;
            if (!memoizedBucket.TryGetValue(pixel, out bestBucketIndex))
            {
                double bestDistance = double.MaxValue;
                for (int i = 0; i < histogram.Results.Length; i++)
                {
                    Histogram.Entry entry = histogram.Results[i];
                    double distance = CalculateDistanceSquared(Conversion.Lab.Unpack(entry.Bucket), color);
                    if (distance < bestDistance)
                    {
                        bestBucketIndex = i;
                        bestDistance = distance;
                    }
                }

                memoizedBucket[pixel] = bestBucketIndex;
            }

            Histogram.Entry bestEntry = histogram.Results[bestBucketIndex];
            bestEntry.Average = Conversion.Lab.Pack(UpdateMeanColor((Conversion.Lab.Unpack(bestEntry.Average), bestEntry.Count), color).Item1);
            bestEntry.Count++;
        }

        //var maxes = histogram.Results.OrderBy(h => h.Count).Where(h => h.Count > 0).Take(16).ToList();
        var maxes = histogram.Results.OrderByDescending(h => h.Count).Take(16).ToList();
        List<IMagickColor<byte>> palette = maxes.Select(entry =>
        {
            var color = Conversion.Lab.Unpack(entry.Average);
            var (h, s, v) = new Unicolour(ColourSpace.Lab, color.L, color.A, color.B).Hsb;
            return new SimpleColor.Hsv(h / 360, s, v);
        }).OrderBy(c => c, new SimpleColor.HsvComparer()).Select(c => new ColorHSV(c.H, c.S, c.V).ToMagickColor()).ToList();

        return palette;
    }

    /// <summary>
    /// Creates Palette from pixels using Histogram
    /// </summary>
    /// 
    /// <param name="pixels">The pixels of the image in HSV space.</param>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromPixels(SimpleColor.Rgb[] pixels, Dictionary<SimpleColor.Rgb, SimpleColor.PackedHsv> colormap, Tolerances tolerances, Buckets buckets)
    {
        SortedDictionary<SimpleColor.Hsv, int> histogram = new(new ThresholdHsvComparer(tolerances));
        foreach (var pixel in pixels)
        {
            SimpleColor.Hsv color = Conversion.Hsv.Unpack(colormap[pixel]);
            if (histogram.ContainsKey(color))
            {
                histogram[color]++;
            }
            else
            {
                histogram.Add(color, 1);
            }
        }

        // foreach (var item in histogram)
        // {
        //     Console.WriteLine($"hist: {item.Key} count: {item.Value}");
        // }

        //var maxes = histogram.Results.OrderBy(h => h.Count).Where(h => h.Count > 0).Take(16).ToList();
        var maxes = histogram.OrderByDescending(h => h.Value).Take(16).ToList();
        List<IMagickColor<byte>> palette = maxes.Select(entry => entry.Key).OrderBy(c => c, new SimpleColor.HsvComparer()).Select(c => new ColorHSV(c.H, c.S, c.V).ToMagickColor()).ToList();

        return palette;
    }


    private static List<IMagickColor<byte>> FromImageDirect(IMagickImage<byte> image, Tolerances tolerances, Buckets buckets)
    {
        SimpleColor.Rgb[] pixels = new SimpleColor.Rgb[image.Width * image.Height];
        
        Dictionary<SimpleColor.Rgb, SimpleColor.PackedLab> colormap = [];
        int i = 0;
        foreach(var pixelBytes in image.GetPixelBytes())
        {
            for (uint j = 0; j < pixelBytes.Length; j += image.ChannelCount)
            {
                SimpleColor.Rgb pixel = new(pixelBytes[j], pixelBytes[j + 1], pixelBytes[j + 2]);
                if (!colormap.ContainsKey(pixel))
                {
                    var (h, s, b) = new Unicolour(ColourSpace.Rgb255, pixel.R, pixel.G, pixel.B).Lab;
                    colormap[pixel] = Conversion.Lab.Pack(h, s, b);
                }

                pixels[i] = pixel;
                i++;
            }
        }
        
        return FromPixels(pixels, colormap, tolerances, buckets);
    }

    /// <summary>
    /// Generates a palette using a Histogram.
    /// </summary>
    /// <param name="image">The image to generate from.</param>
    /// <param name="tolerances">The tolerances that represent threshold for histogram to find a match.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromImage(MagickImage image, Tolerances tolerances, Buckets buckets)
    {
        double largePixels = 640 * 480;
        double imageLength = image.Width * image.Height;

        if (imageLength > largePixels)
        {
            //Console.WriteLine("resized");
            using var sample = image.Clone();
            sample.Sample(new Percentage(100 / Math.Sqrt(imageLength / largePixels)));
            sample.Write("out-thing.png");
            return FromImageDirect(sample, tolerances, buckets);
        }
        
        return FromImageDirect(image, tolerances, buckets);
    }

    /// <summary>
    /// Generates one K-Means Cluster from pixels.
    /// </summary>
    /// <param name="pixels">The pixels of the image in LAB space.</param>
    /// <param name="clusters">Previous cluster values.</param>
    /// <returns></returns>
    private static SimpleColor.Lab[] KMeansCluster(SimpleColor.Rgb[] pixels, SimpleColor.Lab[] clusters, Dictionary<SimpleColor.Rgb, SimpleColor.PackedLab> colormap)
    {
        // Total colors in an image is usually a lot less than the number of pixels.
        Dictionary<SimpleColor.Rgb, int> memoizedCluster = [];

        (SimpleColor.Lab, int)[] means = clusters.Select(c => (c, 0)).ToArray();
        foreach (var pixel in pixels)
        {
            SimpleColor.Lab colorSimple = Conversion.Lab.Unpack(colormap[pixel]);

            int bestClusterIndex = 0;
            if (!memoizedCluster.TryGetValue(pixel, out bestClusterIndex))
            {
                double bestDistance = double.MaxValue;
                for (int i = 0; i < clusters.Length; i++)
                {
                    double distance = CalculateDistanceSquared(clusters[i], colorSimple);
                    if (distance < bestDistance)
                    {
                        bestClusterIndex = i;
                        bestDistance = distance;
                    }
                }

                memoizedCluster[pixel] = bestClusterIndex;
            }
            
            means[bestClusterIndex] = UpdateMeanColor(means[bestClusterIndex], colorSimple);
        }

        return means.Select(m =>
        {
            var (mean, _) = m;
            return mean;
        }).ToArray();
    }

     /// <summary>
    /// Generates one K-Means Cluster from pixels.
    /// </summary>
    /// <param name="pixels">The pixels of the image in LAB space.</param>
    /// <param name="clusters">Previous cluster values.</param>
    /// <returns></returns>
    private static SimpleColor.Lab[] KMeansClusterParalell(SimpleColor.Rgb[] pixels, SimpleColor.Lab[] clusters, Dictionary<SimpleColor.Rgb, SimpleColor.PackedLab> colormap)
    {
        // Total colors in an image is usually a lot less than the number of pixels.
        //Dictionary<SimpleColor.Rgb, int> memoizedCluster = [];

        //(SimpleColor.Lab, int)[] means = clusters.Select(c => (c, 0)).ToArray();
        ConcurrentBag<(SimpleColor.Lab, int)[]> bag = new();
        Parallel.ForEach(
            pixels,
            () => (new Dictionary<SimpleColor.Rgb, int>(), clusters.Select(c => (c, 0)).ToArray()), 
            (pixel, _, threadLocals) =>
            {
                var (memoizedCluster, means) = threadLocals;
                SimpleColor.Lab colorSimple = Conversion.Lab.Unpack(colormap[pixel]);

                int bestClusterIndex = 0;
                if (!memoizedCluster.TryGetValue(pixel, out bestClusterIndex))
                {
                    double bestDistance = double.MaxValue;
                    for (int i = 0; i < clusters.Length; i++)
                    {
                        double distance = CalculateDistanceSquared(clusters[i], colorSimple);
                        if (distance < bestDistance)
                        {
                            bestClusterIndex = i;
                            bestDistance = distance;
                        }
                    }

                    memoizedCluster[pixel] = bestClusterIndex;
                }
        
                means[bestClusterIndex] = UpdateMeanColor(means[bestClusterIndex], colorSimple);

                return (memoizedCluster, means);
            }, 
            threadLocals =>
            {
                var (_, means) = threadLocals;
                bag.Add(means);
            }
        );


        var means = clusters.Select(c => (c, 0)).ToArray();
        foreach(var threadMeans in bag)
        {
            means = means.Zip(threadMeans).Select(z => 
                {
                    ((SimpleColor.Lab mean, int count) total, (SimpleColor.Lab mean, int count) thread) = z;
                    return UpdateMeanColor(total, thread);
                }
            ).ToArray();
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
    public static List<IMagickColor<byte>> FromPixelsKmeans(SimpleColor.Rgb[] pixels, List<IMagickColor<byte>> seeds, Dictionary<SimpleColor.Rgb, SimpleColor.PackedLab> colormap, bool verbose = false)
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

            SimpleColor.Lab[] newClusters = KMeansClusterParalell(pixels, clusters, colormap);

            // If there is not a lot of change based on epsilon value then stop iterating.
            finished = clusters.Zip(newClusters).All(pair =>
            {
                var (oldCluster, newCluster) = pair;
                return CalculateDistance(oldCluster, newCluster) <= 2.0;
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
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromImageKmeans(MagickImage image, List<IMagickColor<byte>> seeds, bool verbose = false)
    {
        // pixels could be extremely large if the image is 4K or higher. But it only has 3 bytes each.
        // colormap is size limited by RGB values and is usually a lot less than pixels.
        SimpleColor.Rgb[] pixels = new SimpleColor.Rgb[image.Width * image.Height];
        Dictionary<SimpleColor.Rgb, SimpleColor.PackedLab> colormap = [];
        
        int i = 0;
        foreach(var pixelBytes in image.GetPixelBytes())
        {
            for (uint j = 0; j < pixelBytes.Length; j += image.ChannelCount)
            {
                SimpleColor.Rgb pixel = new(pixelBytes[j], pixelBytes[j + 1], pixelBytes[j + 2]);
                if (!colormap.ContainsKey(pixel))
                {
                    var (l, a, b) = new Unicolour(ColourSpace.Rgb255, pixel.R, pixel.G, pixel.B).Lab;
                    colormap[pixel] = Conversion.Lab.Pack(l, a, b);
                }

                pixels[i] = pixel;
                i++;
            }
        }

        //Console.WriteLine("K means ready");

        return FromPixelsKmeans(pixels, seeds, colormap, verbose);
    }
}