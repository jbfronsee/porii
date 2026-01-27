using ImageMagick;

using App.Extensions;
using App.Io;
using Lib.Analysis;
using Lib.Analysis.Interfaces;
using Lib.Colors;

namespace App.Core;

public static class Palette
{
    private static IMagickImage<byte> GetCopiedSample(IMagickImage<byte> image, double largePixelCount)
    {
        double imageLength = image.Width * image.Height;
        
        IMagickImage<byte> sample = image.Clone();
        sample.Sample(new Percentage(100 / Math.Sqrt(imageLength / largePixelCount)));
        return sample;
    }

    /// <summary>
    /// Creates Palette from pixels using Histogram
    /// </summary>
    /// 
    /// <param name="pixels">The pixels of the image in HSV space.</param>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static IHistogramLab CalculateHistogramFromPixels(ColorRgb[] pixels, Dictionary<ColorRgb, PackedLab> colormap, Buckets buckets)
    {
        KMeansHistogramLab histogram = new(colormap);

        int j = 0;
        foreach (var bucket in buckets.PaletteLab())
        {
            VectorLab labBucket = bucket;
            EntryLab entry = new(labBucket, labBucket, 0);
            histogram.Results[j] = entry;
            j++;
        }

        histogram.CalculateHistogram(pixels);

        return histogram;
    }

    public static IHistogramLab CalculateHistogram(IMagickImage<byte> image, Buckets buckets)
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
    public static IHistogramLab CalculateHistogramFromSample(IMagickImage<byte> image, Buckets buckets)
    {
        double largePixels = 640 * 480;
        double imageLength = image.Width * image.Height;

        if (imageLength > largePixels)
        {
            using var sample = GetCopiedSample(image, largePixels);
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
    public static List<IMagickColor<byte>> FromPixelsKmeans(
        ColorRgb[] pixels,
        List<IMagickColor<byte>> seeds,
        Dictionary<ColorRgb, PackedLab> colormap,
        bool parallelize = false,
        bool verbose = false
    )
    { 
        KMeansLab kmeans = new(seeds.Select(Colors.Convert.ToLab).Select(c => new SafeClusterLab(c, c, 0)).ToArray(), colormap);

        Format.WriteLineIf(verbose, $"K-Means Cluster Index: ");

        foreach (var index in kmeans.BestClustersWithProgress(pixels, 32, parallelize))
        {
            Format.WriteLineIf(verbose, $"{index}");
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
    public static List<IMagickColor<byte>> FromImageKmeans(
        IMagickImage<byte> image,
        List<IMagickColor<byte>> seeds,
        Dictionary<ColorRgb, PackedLab>? colormap = null,
        bool parallelize = false,
        bool verbose = false
    )
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

        return FromPixelsKmeans(pixels, seeds, colormap, parallelize, verbose);
    }

    /// <summary>
    /// Generates a palette using K-Means Clustering.
    /// </summary>
    /// <param name="image">The image to generate from.</param>
    /// <param name="seeds">The seed values to make initial clusters from.</param>
    /// <param name="verbose">Flag that enables printing K-Means progress message.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    /// <returns>The palette as a list of MagickColors ordered by Hue then Saturation then Value.</returns>
    public static List<IMagickColor<byte>> FromImage(
        IMagickImage<byte> image,
        List<IMagickColor<byte>> seeds,
        double largePixelCount,
        Dictionary<ColorRgb, PackedLab>? colormap = null,
        bool verbose = false
    )
    {
        double imageLength = image.Width * image.Height;

        if (imageLength > largePixelCount)
        {
            using var sample = GetCopiedSample(image, largePixelCount);
            return FromImageKmeans(sample, seeds, colormap, true, verbose);
        }
        
        return FromImageKmeans(image, seeds, colormap, true, verbose);
    }
}
