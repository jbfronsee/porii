public static class LabConverter
{

    // Calculated ranges:
    // Min: Lab { L = 0, A = -85.92633756637852, B = -107.5392995774092 }
    // Max: Lab { L = 99.65492223276893, A = 97.94211021307359, B = 94.19690948640105 }
    private const double LScale = byte.MaxValue / 100;

    private const double AScale = byte.MaxValue / (86 + 98);

    private const double BScale = byte.MaxValue / (108 + 94);

    private const int AShift = 86;

    private const int BShift = 108;

    private static byte LToByte(double l)
    {
        return (byte)(l * LScale);
    }

    private static byte AToByte(double a)
    {
        
        return (byte)((a + AShift) * AScale);
    }

    private static byte BToByte(double b)
    {
        return (byte)((b + BShift) * BScale);
    }

    private static double LToDouble(byte l)
    {
        return l / LScale;
    }

    private static double AToDouble(byte a)
    {
        
        return (a / AScale) - AShift;
    }

    private static double BToDouble(byte b)
    {
        return (b / BScale) - BShift;
    }

    /// <summary>
    /// Convert Lab value from double representation to byte representation.
    /// Precision is lost during conversion.
    /// </summary>
    /// <param name="l">l</param>
    /// <param name="a">a</param>
    /// <param name="b">b</param>
    /// <returns></returns>
    public static ByteColor.Lab ToByte(double l, double a, double b)
    {
        return new ByteColor.Lab(LToByte(l), AToByte(a), BToByte(b));
    }

    /// <summary>
    /// Convert Lab value from double representation to byte representation.
    /// Precision is lost during conversion.
    /// </summary>
    /// <param name="lab">Color to convert.</param>
    /// <returns></returns>
    public static ByteColor.Lab ToByte(SimpleColor.Lab lab)
    {
        return ToByte(lab.L, lab.A, lab.B);
    }

    /// <summary>
    /// Convert Lab value from byte representation to double representation.
    /// </summary>
    /// <param name="l">l</param>
    /// <param name="a">a</param>
    /// <param name="b">b</param>
    /// <returns></returns>
    public static SimpleColor.Lab ToSimple(byte l, byte a, byte b)
    {
        return new SimpleColor.Lab(LToDouble(l), AToDouble(a), BToDouble(b));
    }

    /// <summary>
    /// Convert Lab value from byte representation to double representation.
    /// </summary>
    /// <param name="lab">Color to convert.</param>
    /// <returns></returns>
    public static SimpleColor.Lab ToSimple(ByteColor.Lab lab)
    {
        return ToSimple(lab.L, lab.A, lab.B);
    }
}