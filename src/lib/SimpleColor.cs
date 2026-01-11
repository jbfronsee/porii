namespace SimpleColor
{
    /// <summary>
    /// A 3 double struct representing an HSV Colorspace Value.
    /// </summary>
    /// <param name="H">Hue</param>
    /// <param name="S">Saturation</param>
    /// <param name="V">Value</param>
    public record struct Hsv(double H, double S, double V);

    /// <summary>
    /// A 3 double struct representing an Lab Colorspace Value.
    /// </summary>
    /// <param name="L">L</param>
    /// <param name="A">a</param>
    /// <param name="B">b</param>
    public record struct Lab(double L, double A, double B);

    // Records implement IEquatable but not IComparable

    /// <summary>
    /// HSV Comparer for sorting by Hue, Saturation, and Value in that order.
    /// </summary>
    public class HsvComparer() : IComparer<Hsv>
    {
        public virtual int Compare(Hsv x, Hsv y)
        {
            int result = x.H.CompareTo(y.H);
            if (result != 0) return result;

            result = x.S.CompareTo(y.S);
            if (result != 0) return result;

            return x.V.CompareTo(y.V);
        }
    }

    /// <summary>
    /// Lab Comparer for sorting by L, a, and b, in that order.
    /// </summary>
    public class LabComparer() : IComparer<Lab>
    {
        public virtual int Compare(Lab x, Lab y)
        {
            int result = x.L.CompareTo(y.L);
            if (result != 0) return result;

            result = x.A.CompareTo(y.A);
            if (result != 0) return result;

            return x.B.CompareTo(y.B);
        }
    }
}