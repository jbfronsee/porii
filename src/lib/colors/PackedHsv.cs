using Lib.Colors.Interfaces;

public record struct PackedHsv(ushort H, ushort S, ushort V) : IPacked<ColorHsv>
{
    private static ushort PackDouble(double value)
    {
        return (ushort)(value * ushort.MaxValue);
    }

    private static double UnpackShort(ushort value)
    {
        return (double)value / ushort.MaxValue;
    }

    /// <summary>
    /// Convert HSV value from double representation to byte representation.
    /// 
    /// The calculated number of colors it can represent is all 16,777,216 RGB colors
    /// of the total RGB values.
    /// </summary>
    /// <param name="hsv">Color to convert.</param>
    /// <returns></returns>
    public static PackedHsv Pack(ColorHsv hsv)
    {
        return new PackedHsv(PackDouble(hsv.H), PackDouble(hsv.S), PackDouble(hsv.V));
    }

    /// <summary>
    /// Convert HSV value from byte representation to double representation.
    /// </summary>
    /// <param name="h">Hue</param>
    /// <param name="s">Saturation</param>
    /// <param name="v">Value</param>
    /// <returns></returns>
    public static ColorHsv Unpack(PackedHsv hsv)
    {
        return new ColorHsv(UnpackShort(hsv.H), UnpackShort(hsv.S), UnpackShort(hsv.V));
    }

    public readonly ColorHsv Unpack()
    {
        return Unpack(this);
    }
}