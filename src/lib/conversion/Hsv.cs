
namespace Lib.Conversion;

public static class Hsv
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
    /// Convert HSV value from double representation to ushort representation.
    /// 
    /// The calculated number of colors it can represent is all 16,777,216 RGB colors
    /// of the total RGB values.
    /// </summary>
    /// <param name="h">Hue</param>
    /// <param name="s">Saturation</param>
    /// <param name="v">Value</param>
    /// <returns></returns>
    public static SimpleColor.PackedHsv Pack(double h, double s, double v)
    {
        return new SimpleColor.PackedHsv(PackDouble(h), PackDouble(s), PackDouble(v));
    }

    /// <summary>
    /// Convert HSV value from double representation to byte representation.
    /// 
    /// The calculated number of colors it can represent is all 16,777,216 RGB colors
    /// of the total RGB values.
    /// </summary>
    /// <param name="hsv">Color to convert.</param>
    /// <returns></returns>
    public static SimpleColor.PackedHsv Pack(SimpleColor.Hsv hsv)
    {
        return Pack(hsv.H, hsv.S, hsv.V);
    }

    /// <summary>
    /// Convert HSV value from byte representation to double representation.
    /// </summary>
    /// <param name="h">Hue</param>
    /// <param name="s">Saturation</param>
    /// <param name="v">Value</param>
    /// <returns></returns>
    public static SimpleColor.Hsv Unpack(ushort h, ushort s, ushort v)
    {
        return new SimpleColor.Hsv(UnpackShort(h), UnpackShort(s), UnpackShort(v));
    }

    /// <summary>
    /// Convert HSV value from byte representation to double representation.
    /// </summary>
    /// <param name="hsv">Color to convert.</param>
    /// <returns></returns>
    public static SimpleColor.Hsv Unpack(SimpleColor.PackedHsv hsv)
    {
        return Unpack(hsv.H, hsv.S, hsv.V);
    }
}
