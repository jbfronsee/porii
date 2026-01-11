namespace ByteColor
{
    public record struct Hsv(byte H, byte S, byte V);

    public record struct Lab(byte L, byte A, byte B);

    // Records implement IEquatable but not IComparable
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