using Wacton.Unicolour;

using Lib.Colors;

namespace App.Core;

public class Buckets
{
    public Buckets(List<double>? saturatedHues = null, List<double>? desaturatedHues = null)
    {
        SaturatedHues = saturatedHues ?? [];
        DesaturatedHues = desaturatedHues ?? [];
    }

    public enum Group
    {
        Saturated,
        Desaturated,
        Grayscale
    }

    public List<double> SaturatedHues { get; set; }

    public List<double> DesaturatedHues { get; set; }

    public IEnumerable<Unicolour> Interpolate()
    {
        List<Unicolour> palette = [];
        foreach (double hue in SaturatedHues)
        {
            Unicolour start = new(ColourSpace.Hsb, hue, 1, .9);
            Unicolour end = new(ColourSpace.Hsb, hue, 1, .1);
            palette.AddRange(start.Palette(end, ColourSpace.Hsb, 12));
        }

        foreach (double hue in DesaturatedHues)
        {
            Unicolour start = new(ColourSpace.Hsb, hue, .3, .9);
            Unicolour end = new(ColourSpace.Hsb, hue, .5, .20);
            palette.AddRange(start.Palette(end, ColourSpace.Hsb, 8));
        }

        Unicolour grayStart = new(ColourSpace.Hsb, 0, 0, 1);
        Unicolour grayEnd = new(ColourSpace.Hsb, 0, 0, 0);
        palette.AddRange(grayStart.Palette(grayEnd, ColourSpace.Hsb, 32));

        return palette;
    }

    public IEnumerable<ColorHsv> PaletteHsv()
    {
        return Interpolate().Select(u => u.Hsb).Select(c => new ColorHsv(c.H / 360, c.S, c.B));
    }

    public IEnumerable<VectorLab> PaletteLab()
    {
        return Interpolate().Select(u => u.Lab).Select(c => new VectorLab(c.L, c.A, c.B));
    }

    public IEnumerable<ColorRgb> PaletteRgb()
    {

        return Interpolate().Select(u => u.Rgb.Byte255).Select(c => new ColorRgb((byte)c.R, (byte)c.G, (byte)c.B));
    }

    public (bool, string) Validate()
    {
        if (SaturatedHues.Count != 12)
        {
            return (false, "Please specificy 12 Hues that will represent Histogram saturated hue ranges.");
        }

        if (DesaturatedHues.Count != 10)
        {
            return (false, "Please specificy 12 Hues that will represent Histogram saturated hue ranges.");
        }

        return (true, "");
    }
}
