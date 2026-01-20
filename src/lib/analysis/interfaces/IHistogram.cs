namespace Lib.Analysis.Interfaces;

public interface IHistogram<T>
{
    public T[] Results { get; }

    public int TotalPixelsCounted { get; set; }

    public void CalculateHistogram(ColorRgb[] pixels);

    public void CalculateHistogramParallel(ColorRgb[] pixels);

    public List<T> Palette();

    public List<T> PaletteWithFilter(FilterStrength filter = FilterStrength.Medium);
}
