public static class HsvConverter
{
    private static byte ToByte(double value)
    {
        return (byte)(value * byte.MaxValue);
    }

    private static double ToSimple(byte value)
    {
        return (double)value / byte.MaxValue;
    }

    public static ByteColor.Hsv ToByte(double h, double s, double v)
    {
        return new ByteColor.Hsv(ToByte(h), ToByte(s), ToByte(v));
    }

    public static ByteColor.Hsv ToByte(SimpleColor.Hsv hsv)
    {
        return ToByte(hsv.H, hsv.S, hsv.V);
    }

    public static SimpleColor.Hsv ToSimple(byte h, byte s, byte v)
    {
        return new SimpleColor.Hsv(ToSimple(h), ToSimple(s), ToSimple(v));
    }

    public static SimpleColor.Hsv ToSimple(ByteColor.Hsv hsv)
    {
        return ToSimple(hsv.H, hsv.S, hsv.V);
    }
}