/// <summary>
/// A 3 byte struct representing an RGB Colorspace Value.
/// </summary>
/// <param name="R">Red</param>
/// <param name="G">Green</param>
/// <param name="B">Blue</param>
public record struct ColorRgb(byte R, byte G, byte B) : IComparable<ColorRgb>
{
    public int CompareTo(ColorRgb other)
    {
        int result = R.CompareTo(other.R);
        if (result != 0) return result;

        result = G.CompareTo(other.G);
        if (result != 0) return result;

        return B.CompareTo(other.B);
    }
}
