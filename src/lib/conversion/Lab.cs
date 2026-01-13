namespace Lib.Conversion;

public static class Lab
{
    // Values were calculated using every 8-bit RGB value.

    private const double ARange = AMax - AMin;

    private const double BRange = BMax - BMin;

    private const double LScale = ushort.MaxValue / LMax;

    private const double AScale = ushort.MaxValue / ARange;

    private const double BScale = ushort.MaxValue / BRange;
    
    //////////////////////
    // Public Constants //
    //////////////////////

    public const double LMin = 0;

    public const double LMax = 100;

    public const double AMin = -86.18271462445199;

    public const double AMax = 98.23433854246716;

    public const double BMin = -107.86016288933186;

    public const double BMax = 94.47796331945983;

    private static double ABPack(double val, double scale, double min)
    {
        return (val - min) * scale;
    }

    private static ushort LPack(double l)
    {
        return (ushort)(l * LScale);
    }

    private static ushort APack(double a)
    {
        
        return (ushort)ABPack(a, AScale, AMin);
    }

    private static ushort BPack(double b)
    {
        return (ushort)ABPack(b, BScale, BMin);
    }

    private static double ABUnpack(double val, double scale, double min)
    {
        
        return (val / scale) + min;
    }

    private static double LUnpack(ushort l)
    {
        return l / LScale;
    }

    private static double AUnpack(ushort a)
    {
        
        return ABUnpack(a, AScale, AMin);
    }

    private static double BUnpack(ushort b)
    {
        return ABUnpack(b, BScale, BMin);
    }

    /// <summary>
    /// Convert Lab value from double representation to ushort representation.
    /// 
    /// Precision of Lab space is lost during conversion but the calculated
    /// total RGB values that it can map is all 16,777,216 possible values.
    /// </summary>
    /// <param name="l">l</param>
    /// <param name="a">a</param>
    /// <param name="b">b</param>
    /// <returns></returns>
    public static SimpleColor.PackedLab Pack(double l, double a, double b)
    {
        return new SimpleColor.PackedLab(LPack(l), APack(a), BPack(b));
    }

    /// <summary>
    /// Convert Lab value from double representation to byte representation.
    /// 
    /// Precision of Lab space is lost during conversion but the calculated
    /// total RGB values that it can map is all 16,777,216 possible values.
    /// </summary>
    /// <param name="lab">Color to convert.</param>
    /// <returns></returns>
    public static SimpleColor.PackedLab Pack(SimpleColor.Lab lab)
    {
        return Pack(lab.L, lab.A, lab.B);
    }

    /// <summary>
    /// Convert Lab value from ushort representation to double representation.
    /// </summary>
    /// <param name="l">l</param>
    /// <param name="a">a</param>
    /// <param name="b">b</param>
    /// <returns></returns>
    public static SimpleColor.Lab Unpack(ushort l, ushort a, ushort b)
    {
        return new SimpleColor.Lab(LUnpack(l), AUnpack(a), BUnpack(b));
    }

    /// <summary>
    /// Convert Lab value from ushort representation to double representation.
    /// </summary>
    /// <param name="lab">Color to convert.</param>
    /// <returns></returns>
    public static SimpleColor.Lab Unpack(SimpleColor.PackedLab lab)
    {
        return Unpack(lab.L, lab.A, lab.B);
    }
}
