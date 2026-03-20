using Lib.Colors.Interfaces;

namespace Lib.Analysis.Interfaces;

public interface IHistogram<T, U>
    where T: class, IEntry<U, T>
    where U: IColorVector<double>, IEquatable<U>
{
    public T[] Results { get; }

    public int TotalPixelsCounted { get; set; }

    public void CalculateHistogram(ColorRgb[] pixels);

    public void CalculateHistogramParallel(ColorRgb[] pixels);

    public List<T> UniqueEntries();

    public List<T> FilteredEntries(FilterStrength filter = FilterStrength.Medium);

    public List<U> FilteredPalette(FilterStrength strength = FilterStrength.Medium);
}
