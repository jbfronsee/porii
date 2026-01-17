using SimpleColor = Lib.SimpleColor;
using App.Extensions;
using ImageMagick;
using ImageMagick.Colors;
using Lib.Analysis;
using Lib.Colors;


namespace App.Core;

public static class Palette
{
    /// <summary>
    /// Creates Palette from pixels using Histogram
    /// </summary>
    /// 
    /// <param name="pixels">The pixels of the image in HSV space.</param>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromPixels(SimpleColor.Rgb[] pixels, Dictionary<SimpleColor.Rgb, PackedLab> colormap, Tolerances tolerances, Buckets buckets)
    {
        Histogram histogram = new();

        int j = 0;
        foreach (var bucket in buckets.PaletteRgb())
        {
            VectorLab labBucket = Colors.Convert.ToLab(bucket);
            Histogram.Entry entry = new(labBucket, labBucket, 0);
            histogram.Results[j] = entry;
            j++;
        }

        histogram.Cluster(pixels, colormap);

        var maxes = histogram.Results.OrderByDescending(h => h.Count).Take(16).ToList();
        List<IMagickColor<byte>> palette = maxes
            .Select(entry => Colors.Convert.ToHsv(entry.Mean))
            .OrderBy(c => c)
            .Select(c => new ColorHSV(c.H, c.S, c.V).ToMagickColor())
            .ToList();

        return palette;
    }


    private static List<IMagickColor<byte>> FromImageDirect(IMagickImage<byte> image, Tolerances tolerances, Buckets buckets)
    {
        SimpleColor.Rgb[] pixels = new SimpleColor.Rgb[image.Width * image.Height];
        
        Dictionary<SimpleColor.Rgb, PackedLab> colormap = [];
        int i = 0;
        foreach(var pixelBytes in image.GetPixelBytes())
        {
            for (uint j = 0; j < pixelBytes.Length; j += image.ChannelCount)
            {
                SimpleColor.Rgb pixel = new(pixelBytes[j], pixelBytes[j + 1], pixelBytes[j + 2]);
                if (!colormap.ContainsKey(pixel))
                {
                    colormap[pixel] = Colors.Convert.ToLab(pixel).Pack();
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
        //double largePixels = 1920 * 1080;
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
    /// Generates a palette using K-Means Clustering from an array of LAB space pixels.
    /// </summary>
    /// <param name="pixels">The pixels of the image in LAB space.</param>
    /// <param name="seeds">The seed values to make initial clusters from.</param>
    /// <param name="verbose">Flag that enables printing K-Means progress message.</param>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromPixelsKmeans(SimpleColor.Rgb[] pixels, List<IMagickColor<byte>> seeds, Dictionary<SimpleColor.Rgb, PackedLab> colormap, bool verbose = false)
    {        
        int maxIterations = 32;
        VectorLab[] clusters = seeds.Select(Colors.Convert.ToLab).ToArray();

        KMeansLab kmeans = new(clusters.Select(c => new ClusterLab(c, c, 0)).ToArray());

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

            kmeans.ClusterParallel(pixels, colormap);

            // If there is not a lot of change based on epsilon value then stop iterating.
            finished = clusters.Zip(kmeans.Clusters).All(pair =>
            {
                var (oldCluster, newCluster) = pair;
                return ColorMath.CalculateDistance(oldCluster, newCluster.Mean) <= 2.0;
            });

            clusters = kmeans.Clusters.Select(c => c.Mean).ToArray();
        }

        List<SimpleColor.Hsv> palette = clusters.Select(Colors.Convert.ToHsv).ToList();
        palette.Sort();
        
        return palette.Select(Colors.Convert.ToMagickColor).ToList();
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
        Dictionary<SimpleColor.Rgb, PackedLab> colormap = [];
        
        int i = 0;
        foreach(var pixelBytes in image.GetPixelBytes())
        {
            for (uint j = 0; j < pixelBytes.Length; j += image.ChannelCount)
            {
                SimpleColor.Rgb pixel = new(pixelBytes[j], pixelBytes[j + 1], pixelBytes[j + 2]);
                if (!colormap.ContainsKey(pixel))
                {
                    colormap[pixel] = Colors.Convert.ToLab(pixel).Pack();
                }

                pixels[i] = pixel;
                i++;
            }
        }

        //Console.WriteLine("K means ready");

        return FromPixelsKmeans(pixels, seeds, colormap, verbose);
    }
}