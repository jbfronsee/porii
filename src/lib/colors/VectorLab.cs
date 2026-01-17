namespace Lib.Colors;

 /// <summary>
/// A 3 double struct representing an Lab Colorspace Value.
/// </summary>
/// <param name="L">L</param>
/// <param name="A">a</param>
/// <param name="B">b</param>
public record struct VectorLab(double L, double A, double B) : IComparable<VectorLab>, IColorVector<double>, IPackable<PackedLab>
{
    public double X { readonly get => L; set => L = value; }

    public double Y { readonly get => A; set => A = value; }

    public double Z { readonly get => B; set => B = value; }
    
    public readonly int CompareTo(VectorLab other)
    {
        int result = L.CompareTo(other.L);
        if (result != 0) return result;

        result = A.CompareTo(other.A);
        if (result != 0) return result;

        return B.CompareTo(other.B);
    }

    public readonly PackedLab Pack()
    {
        return PackedLab.Pack(this);
    }
}