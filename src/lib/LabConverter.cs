public static class LabConverter
{
    private const double LScale = byte.MaxValue / 100;

    private const int ABShift = 128;

    private static byte LToByte(double l)
    {
        return (byte)(l * LScale);
    }

    private static byte ABToByte(double ab)
    {
        return (byte)(ab + ABShift);
    }

    private static double LToSimple(byte l)
    {
        return l / LScale;
    }

    private static double ABToSimple(byte ab)
    {
        return (double)ab - ABShift;
    }

    public static ByteColor.Lab ToByte(double l, double a, double b)
    {
        return new ByteColor.Lab(LToByte(l), ABToByte(a), ABToByte(b));
    }

    public static ByteColor.Lab ToByte(SimpleColor.Lab lab)
    {
        return ToByte(lab.L, lab.A, lab.B);
    }

    public static SimpleColor.Lab ToSimple(byte l, byte a, byte b)
    {
        return new SimpleColor.Lab(LToSimple(l), ABToSimple(a), ABToSimple(b));
    }

    public static SimpleColor.Lab ToSimple(ByteColor.Lab lab)
    {
        return ToSimple(lab.L, lab.A, lab.B);
    }
}