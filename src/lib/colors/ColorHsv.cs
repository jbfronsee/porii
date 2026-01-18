

using Lib.Colors.Interfaces;


/// <summary>
/// A 3 double struct representing an HSV Colorspace Value.
/// </summary>
/// <param name="H">Hue</param>
/// <param name="S">Saturation</param>
/// <param name="V">Value</param>
public record struct ColorHsv(double H, double S, double V) : IComparable<ColorHsv>, IPackable<PackedHsv>
{
    public readonly int CompareTo(ColorHsv other)
    {
        int result = H.CompareTo(other.H);
        if (result != 0) return result;

        result = S.CompareTo(other.S);
        if (result != 0) return result;

        return V.CompareTo(other.V);
    }

    public readonly PackedHsv Pack()
    {
        return PackedHsv.Pack(this);
    }
}
