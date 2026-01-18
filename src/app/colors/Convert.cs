using ImageMagick;
using ImageMagick.Colors;
using Wacton.Unicolour;

using Lib.Colors;

namespace App.Colors;

public static class Convert
{
    public static VectorLab ToLab(ColorRgb rgb)
    {
        var (l, a, b) = new Unicolour(ColourSpace.Rgb255, rgb.R, rgb.G, rgb.B).Lab;
        return new(l, a, b);
    }

    public static VectorLab ToLab(IMagickColor<byte> rgb)
    {
        var (l, a, b) = new Unicolour(ColourSpace.Rgb255, rgb.R, rgb.G, rgb.B).Lab;
        return new(l, a, b);
    }

    public static VectorLab ToLab(ColorHsv hsv)
    {
        var (l, a, b) = new Unicolour(ColourSpace.Hsb, hsv.H * 360, hsv.S, hsv.V).Lab;
        return new((float)l, (float)a, (float)b);
    }

    public static ColorHsv ToHsv(VectorLab lab)
    {
        var (h, s, b) = new Unicolour(ColourSpace.Lab, lab.L, lab.A, lab.B).Hsb;
        return new(h / 360, s, b);
    }

    public static ColorHsv ToHsv(ColorRgb rgb)
    {
        var (h, s, b) = new Unicolour(ColourSpace.Rgb255, rgb.R, rgb.G, rgb.B).Hsb;
        return new(h / 360, s, b);
    }
    
    public static ColorRgb ToRgb(VectorLab lab)
    {
        var rgb = new Unicolour(ColourSpace.Lab, lab.L, lab.A, lab.B).Rgb.Byte255;
        return new((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
    }

    public static ColorRgb ToRgb(ColorHsv hsv)
    {
        var rgb = new Unicolour(ColourSpace.Hsb, hsv.H * 360, hsv.S, hsv.V).Rgb.Byte255;
        return new((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
    }

    public static IMagickColor<byte> ToMagickColor(ColorHsv hsv)
    {
        return new ColorHSV(hsv.H, hsv.S, hsv.V).ToMagickColor();
    }
}
