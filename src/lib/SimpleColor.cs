using Lib.Colors;

namespace Lib.SimpleColor
{
    /// <summary>
    /// A 3 double struct representing an HSV Colorspace Value.
    /// </summary>
    /// <param name="H">Hue</param>
    /// <param name="S">Saturation</param>
    /// <param name="V">Value</param>
    public record struct Hsv(double H, double S, double V) : IComparable<Hsv>
    {
        public int CompareTo(Hsv other)
        {
            int result = H.CompareTo(other.H);
            if (result != 0) return result;

            result = S.CompareTo(other.S);
            if (result != 0) return result;

            return V.CompareTo(other.V);
        }
    }

    /// <summary>
    /// A 3 double struct representing an Lab Colorspace Value.
    /// </summary>
    /// <param name="L">L</param>
    /// <param name="A">a</param>
    /// <param name="B">b</param>
    public record struct Lab(double L, double A, double B) : IComparable<Lab>, IColorVector<double>
    {
        public double X { get => L; set => L = value; }

        public double Y { get => A; set => A = value; }

        public double Z { get => B; set => B = value; }
        
        public int CompareTo(Lab other)
        {
            int result = L.CompareTo(other.L);
            if (result != 0) return result;

            result = A.CompareTo(other.A);
            if (result != 0) return result;

            return B.CompareTo(other.B);
        }
    }

    /// <summary>
    /// A 3 byte struct representing an RGB Colorspace Value.
    /// </summary>
    /// <param name="R">Red</param>
    /// <param name="G">Green</param>
    /// <param name="B">Blue</param>
    public record struct Rgb(byte R, byte G, byte B);


    /// <summary>
    /// A 6 byte struct representing an HSV Colorspace value using 3 ushort fields.
    /// </summary>
    /// <param name="H">Hue</param>
    /// <param name="S">Saturation</param>
    /// <param name="V">Value</param>
    public record struct PackedHsv(ushort H, ushort S, ushort V);

    /// <summary>
    /// A 6 byte struct representing a Lab Colorspace value using 3 ushort fields.
    /// </summary>
    /// <param name="L">L</param>
    /// <param name="A">a</param>
    /// <param name="B">b</param>
    public record struct PackedLab(ushort L, ushort A, ushort B);

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

    /// <summary>
    /// RGB Comparer for sorting by Red, Green, and Blue, in that order.
    /// </summary>
    public class RgbComparer() : IComparer<Rgb>
    {
        public virtual int Compare(Rgb x, Rgb y)
        {
            int result = x.R.CompareTo(y.R);
            if (result != 0) return result;

            result = x.G.CompareTo(y.G);
            if (result != 0) return result;

            return x.B.CompareTo(y.B);
        }
    }
}