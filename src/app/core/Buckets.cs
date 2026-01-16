using SimpleColor = Lib.SimpleColor;

using Wacton.Unicolour;

namespace App.Core;

public class Buckets
{
    public Buckets(List<BucketPoint>? points = null, List<double>? saturatedHues = null, List<double>? desaturatedHues = null)
    {
        Points = points ?? [];
        SaturatedHues = saturatedHues ?? [];
        DesaturatedHues = desaturatedHues ?? [];
    }

    public enum Group
    {
        Saturated,
        Desaturated,
        Grayscale
    }

    public List<BucketPoint> Points { get; set; }

    public List<double> SaturatedHues { get; set; }

    public List<double> DesaturatedHues { get; set; }

    public IEnumerable<Unicolour> Interpolated()
    {
        //IEnumerable<Unicolour>[] quads = new IEnumerable<Unicolour>[4];
        // for (int j = 0; j < quads.Length && (j + 1) < Points.Count; j++)
        // {
        //     BucketPoint bucket = Points[j], nextBucket = Points[j + 1];
        //     // TODO change the added value at the end 
        //     Unicolour start = new(ColourSpace.Hsb, hue, bucket.Saturation, bucket.Value);
        //     if (j > 0)
        //     {
        //         start = new(ColourSpace.Hsb, hue, bucket.Saturation + .05, bucket.Value + 0.05);
        //     }
        //     Unicolour end = new(ColourSpace.Hsb, hue, nextBucket.Saturation, nextBucket.Value);
            
        //     quads[j] = start.Palette(end, ColourSpace.Hsb, bucket.Bins);
        // }

        // if group saturated

        List<Unicolour> palette = [];
        foreach (double hue in SaturatedHues)
        {
            //double hue = i * (360 / 14);
            Unicolour start = new(ColourSpace.Hsb, hue, 1, .9);
            Unicolour end = new(ColourSpace.Hsb, hue, 1, .1);
            palette.AddRange(start.Palette(end, ColourSpace.Hsb, 12));
        }

        foreach (double hue in DesaturatedHues)
        {
            //double hue = i * (360 / 14);
            Unicolour start = new(ColourSpace.Hsb, hue, .3, 1);
            Unicolour end = new(ColourSpace.Hsb, hue, .5, .25);
            palette.AddRange(start.Palette(end, ColourSpace.Hsb, 8));
        }

        Unicolour grayStart = new(ColourSpace.Hsb, 0, 0, 1);
        Unicolour grayEnd = new(ColourSpace.Hsb, 0, 0, 0);
        palette.AddRange(grayStart.Palette(grayEnd, ColourSpace.Hsb, 32));

        return palette;
    }

    public IEnumerable<Unicolour> Interpolate(double hue)
    {
        IEnumerable<Unicolour>[] quads = new IEnumerable<Unicolour>[4];
        for (int j = 0; j < quads.Length && (j + 1) < Points.Count; j++)
        {
            BucketPoint bucket = Points[j], nextBucket = Points[j + 1];
            // TODO change the added value at the end 
            Unicolour start = new(ColourSpace.Hsb, hue, bucket.Saturation, bucket.Value);
            if (j > 0)
            {
                start = new(ColourSpace.Hsb, hue, bucket.Saturation + .05, bucket.Value + 0.05);
            }
            Unicolour end = new(ColourSpace.Hsb, hue, nextBucket.Saturation, nextBucket.Value);
            
            quads[j] = start.Palette(end, ColourSpace.Hsb, bucket.Bins);
        }

        return quads.SelectMany(u => u);
    }


    public IEnumerable<Unicolour> InterpolateGray(double hue)
    {
        IEnumerable<Unicolour>[] quads = new IEnumerable<Unicolour>[4];
        List<BucketPoint> grayPoints = [
            new BucketPoint(0, 0, 4),
            new BucketPoint(0, .25, 4),
            new BucketPoint(0, .40, 4),
            new BucketPoint(0, .65, 4),
            new BucketPoint(0, .9, 4)
        ];
        for (int j = 0; j < quads.Length && (j + 1) < grayPoints.Count; j++)
        {
            BucketPoint bucket = grayPoints[j], nextBucket = grayPoints[j + 1];
            // TODO change the added value at the end 
            Unicolour start = new(ColourSpace.Hsb, hue, bucket.Saturation, bucket.Value);
            if (j > 0)
            {
                start = new(ColourSpace.Hsb, hue, bucket.Saturation + .05, bucket.Value + 0.05);
            }
            Unicolour end = new(ColourSpace.Hsb, hue, nextBucket.Saturation, nextBucket.Value);
            
            quads[j] = start.Palette(end, ColourSpace.Hsb, bucket.Bins);
        }

        return quads.SelectMany(u => u);
    }

    public IEnumerable<SimpleColor.Hsv> InterpolateHsv(double hue)
    {
        return Interpolate(hue).Select(c => c.Hsb).Select(c => new SimpleColor.Hsv(c.H / 360, c.S, c.B));
    }

    public IEnumerable<SimpleColor.Lab> InterpolateLab(double hue)
    {
        return Interpolate(hue).Select(c => c.Lab).Select(c => new SimpleColor.Lab(c.L, c.A, c.B));
    }

    public IEnumerable<SimpleColor.Rgb> InterpolateRgb(double hue)
    {
        return Interpolate(hue).Select(c => c.Rgb.Byte255).Select(c => new SimpleColor.Rgb((byte)c.R, (byte)c.G, (byte)c.B));
    }


    public IEnumerable<SimpleColor.Hsv> PaletteHsv()
    {
        // List<IEnumerable<SimpleColor.Hsv>> palette = [];
        // for (int i = 0; i < 14; i++)
        // {
        //     double hue = i * (360 / 14);
        //     palette.Add(InterpolateHsv(hue));
        // }
        // for (int i = 0; i < 2; i++)
        // {
        //     double hue = i * 360;
        //     palette.Add(InterpolateGray(hue).Select(c => c.Hsb).Select(c => new SimpleColor.Hsv(c.H / 360, c.S, c.B)));
        // }

        // return palette.SelectMany(c => c);

        return Interpolated().Select(u => u.Hsb).Select(c => new SimpleColor.Hsv(c.H / 360, c.S, c.B));
    }

    public IEnumerable<SimpleColor.Lab> PaletteLab()
    {
        // List<IEnumerable<SimpleColor.Lab>> palette = [];
        // for (int i = 0; i < 14; i++)
        // {
        //     double hue = i * (360 / 14);
        //     palette.Add(InterpolateLab(hue));
        // }
        // for (int i = 0; i < 2; i++)
        // {
        //     double hue = i * 360;
        //     palette.Add(InterpolateGray(hue).Select(c => c.Lab).Select(c => new SimpleColor.Lab(c.L, c.A, c.B)));
        // }

        // return palette.SelectMany(c => c);
        return Interpolated().Select(u => u.Lab).Select(c => new SimpleColor.Lab(c.L, c.A, c.B));
    }


    public IEnumerable<SimpleColor.Rgb> PaletteRgb()
    {
        // List<IEnumerable<SimpleColor.Rgb>> palette = [];
        // for (int i = 0; i < 14; i++)
        // {
        //     double hue = i * (360 / 14);
        //     palette.Add(InterpolateRgb(hue));
        // }
        // for (int i = 0; i < 2; i++)
        // {
        //     double hue = i * 360;
        //     palette.Add(InterpolateGray(hue).Select(c => c.Rgb.Byte255).Select(c => new SimpleColor.Rgb((byte)c.R, (byte)c.G, (byte)c.B)));
        // }
        // return palette.SelectMany(c => c);
        return Interpolated().Select(u => u.Rgb.Byte255).Select(c => new SimpleColor.Rgb((byte)c.R, (byte)c.G, (byte)c.B));
    }

    public (bool, string) Validate()
    {
        if (Points.Count != 5)
        {
            return (false, "Please specificy 5 Bucket Points that will represent Histogram saturation and value ranges.");
        }

        foreach (var point in Points)
        {
            var (valid, message) = point.Validate();
            if (!valid)
            {
                return (valid, message);
            }
        }

        return (true, "");
    }
}