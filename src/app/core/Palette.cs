using ImageMagick;

using App.Extensions;
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
    public static HistogramLab CalculateHistogramFromPixels(ColorRgb[] pixels, Dictionary<ColorRgb, PackedLab> colormap, Buckets buckets)
    {
        HistogramLab histogram = new(colormap);

        int j = 0;
        foreach (var bucket in buckets.PaletteLab())
        {
            VectorLab labBucket = bucket;
            EntryLab entry = new(labBucket, labBucket, 0);
            histogram.Results[j] = entry;
            j++;
        }

        histogram.Cluster(pixels);

        return histogram;
    }


    public static HistogramLab CalculateHistogram(IMagickImage<byte> image, Buckets buckets)
    {
        ColorRgb[] pixels = new ColorRgb[image.Width * image.Height];
        
        Dictionary<ColorRgb, PackedLab> colormap = [];
        int i = 0;
        foreach(var pixelBytes in image.GetPixelBytes())
        {
            for (uint j = 0; j < pixelBytes.Length; j += image.ChannelCount)
            {
                ColorRgb pixel = new(pixelBytes[j], pixelBytes[j + 1], pixelBytes[j + 2]);
                if (!colormap.ContainsKey(pixel))
                {
                    colormap[pixel] = Colors.Convert.ToLab(pixel).Pack();
                }

                pixels[i] = pixel;
                i++;
            }
        }
        
        return CalculateHistogramFromPixels(pixels, colormap, buckets);
    }

    /// <summary>
    /// Generates a palette using a Histogram.
    /// </summary>
    /// <param name="image">The image to generate from.</param>
    /// <param name="tolerances">The tolerances that represent threshold for histogram to find a match.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static HistogramLab CalculateHistogramFromSample(IMagickImage<byte> image, Buckets buckets)
    {
        double largePixels = 640 * 480;
        double imageLength = image.Width * image.Height;

        if (imageLength > largePixels)
        {
            using var sample = image.Clone();
            sample.Sample(new Percentage(100 / Math.Sqrt(imageLength / largePixels)));
            return CalculateHistogram(sample, buckets);
        }
        
        return CalculateHistogram(image, buckets);
    }

    /// <summary>
    /// Generates a palette using K-Means Clustering from an array of LAB space pixels.
    /// </summary>
    /// <param name="pixels">The pixels of the image in LAB space.</param>
    /// <param name="seeds">The seed values to make initial clusters from.</param>
    /// <param name="verbose">Flag that enables printing K-Means progress message.</param>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromPixelsKmeans(ColorRgb[] pixels, List<IMagickColor<byte>> seeds, Dictionary<ColorRgb, PackedLab> colormap, bool verbose = false)
    {        
        int maxIterations = 32;
        KMeansLab kmeans = new(seeds.Select(Colors.Convert.ToLab).Select(c => new ClusterLab(c, c, 0)).ToArray(), colormap);

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

            kmeans.ClusterParallel(pixels);
    
            finished = true;
            foreach (var cluster in kmeans.Clusters)
            {
                finished = finished && (ColorMath.CalculateDistance(cluster.Cluster, cluster.Mean) <= 1.0);
                
                cluster.Cluster = cluster.Mean;
            }
        }

        List<ColorHsv> palette = kmeans.Clusters.Select(c => Colors.Convert.ToHsv(c.Mean)).ToList();
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
    public static List<IMagickColor<byte>> FromImageKmeans(IMagickImage<byte> image, List<IMagickColor<byte>> seeds, Dictionary<ColorRgb, PackedLab>? colormap = null,  bool verbose = false)
    {
        colormap ??= [];

        // pixels could be extremely large if the image is 4K or higher. But it only has 3 bytes each.
        ColorRgb[] pixels = new ColorRgb[image.Width * image.Height];

        int i = 0;
        foreach(var pixelBytes in image.GetPixelBytes())
        {
            for (uint j = 0; j < pixelBytes.Length; j += image.ChannelCount)
            {
                ColorRgb pixel = new(pixelBytes[j], pixelBytes[j + 1], pixelBytes[j + 2]);
                if (!colormap.ContainsKey(pixel))
                {
                    colormap[pixel] = Colors.Convert.ToLab(pixel).Pack();
                }

                pixels[i] = pixel;
                i++;
            }
        }

        return FromPixelsKmeans(pixels, seeds, colormap, verbose);
    }
}
