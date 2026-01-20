using Lib.Analysis.Interfaces;
using Lib.Colors;
using Lib.Colors.Interfaces;

namespace Lib.Analysis;

public enum FilterStrength
{
    Undefined,

    Low,
    
    Medium,

    High
}

public abstract class KMeansHistogram<T, U>: KMeans<T, U>, IHistogram<T>
    where T: class, IEntry<U, T>
    where U: IColorVector<double>, IEquatable<U>
{
    protected const int BucketCount = 256;

    protected const double OriginalEpsilonResolution = 1920 * 1080;

    protected const double LowEpsilon = 2.0e-3 / OriginalEpsilonResolution;

    protected const double MediumEpsilon = 2.0e-2 / OriginalEpsilonResolution;

    protected const double HighEpsilon = 2.0e-1 / OriginalEpsilonResolution;

    public override T[] Clusters { get; set; } = new T[BucketCount];

    public T[] Results => Clusters;

    public int TotalPixelsCounted { get; set; } = 0;

    public void CalculateHistogram(ColorRgb[] pixels)
    {
        TotalPixelsCounted = pixels.Length;
        Cluster(pixels);
    }

    public void CalculateHistogramParallel(ColorRgb[] pixels)
    {
        TotalPixelsCounted = pixels.Length;
        ClusterParallel(pixels);
    }

    protected List<T> Palette(T[] entries)
    {
        List<T> matches = [];
        for (int i = 0, j = 1, k = 2; k < entries.Length; i++, j++, k++)
        {
            int prev = entries[i].Count;
            int curr = entries[j].Count;
            int next = entries[k].Count;

            if (prev < curr && curr > next)
            {
                matches.Add(entries[j]);
            }
        }

        return matches;
    }

    protected double GetFilterEpsilon(FilterStrength strength)
    {
        return strength switch
        {
            FilterStrength.High => HighEpsilon * TotalPixelsCounted,
            FilterStrength.Low => LowEpsilon * TotalPixelsCounted,
            _ => MediumEpsilon * TotalPixelsCounted
        };
    }

    public List<T> Palette()
    {
        return Palette(Results);
    }

    public List<T> PaletteWithFilter(FilterStrength strength = FilterStrength.Medium)
    {
        T[] largeValues = Results.Where(e => e.Count > (TotalPixelsCounted * GetFilterEpsilon(strength))).ToArray();
        return Palette(largeValues);
    }

    public override string ToString()
    {
        string result = $"Total Pixels: {TotalPixelsCounted}";
        foreach (var entry in Results)
        {
            result += $"{entry.Count}\n";
        }

        return result;
    }
}


public class KMeansHistogramLab : KMeansHistogram<EntryLab, VectorLab>, IHistogramLab
{
    public Dictionary<ColorRgb, PackedLab> Colormap { get; set; } = [];

    public KMeansHistogramLab() {}

    public KMeansHistogramLab(Dictionary<ColorRgb, PackedLab> colormap)
    {
        Colormap = colormap;
    }

    protected override VectorLab UnpackPixel(ColorRgb pixel)
    {
        return Colormap[pixel].Unpack();
    }
}
