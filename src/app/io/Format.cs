using ImageMagick;
using ImageMagick.Drawing;

namespace App.Io;

public static class Format
{
    public const string GplHeader = "GIMP Palette";

    public const string GplName = "Name:";

    public const string GplColumns = "Columns:";

    private static readonly string mLineSeparator = new('-', 60);
    
    public static string LineSeparator => mLineSeparator;
    
    /// <summary>
    /// Formats palette as a GPL file for importing into software like GIMP and Krita.
    /// </summary>
    /// 
    /// <param name="palette">The palette to format.</param>
    /// <param name="name">The name of the File.</param>
    public static List<string> AsGpl(List<IMagickColor<byte>> palette, string name)
    {
        // Header
        List<string> gplLines =
        [
            GplHeader,
            $"{GplName} {name}",
            $"{GplColumns} 8",
            "#"
        ];

        // Palette Data
        int i = 0;
        foreach (IMagickColor<byte> color in palette)
        {
            gplLines.Add($"{color.R,3} {color.G,3} {color.B,3}\t#{i}");
            i++;
        }

        return gplLines;
    }

    /// <summary>
    /// Formats palette as a PNG for viewing and sampling from.
    /// </summary>
    /// 
    /// <param name="palette">The palette to format.</param>
    /// <param name="name">The name of the File.</param>
    public static MagickImage AsPng(List<IMagickColor<byte>> palette)
    {
        MagickImage image = new(MagickColors.Transparent, 512, 128);

        Drawables canvas = new();

        double x = 0, y = 0, width = 64, height = 64;
        foreach(IMagickColor<byte> color in palette)
        {
            // Define the rectangle's properties
            canvas
                .StrokeColor(color)
                .FillColor(color)
                .Rectangle(x, y, x + width, y + height);

            x += width;
            if (x >= 512)
            {
                x = 0;
                y += height;
            }
        }

        // Draw the squares onto the image
        canvas.Draw(image);
        image.Format = MagickFormat.Png;
        return image;
    }

    public static MagickImage AsPng2(List<IMagickColor<byte>> palette)
    {
        MagickImage image = new(MagickColors.Transparent, 2048, 2048);

        Drawables canvas = new();

        double x = 0, y = 0, width = 128, height = 128;
        foreach(IMagickColor<byte> color in palette)
        {
            // Define the rectangle's properties
            canvas
                .StrokeColor(color)
                .FillColor(color)
                .Rectangle(x, y, x + width, y + height);

            x += width;
            if (x >= 2048)
            {
                x = 0;
                y += height;
            }
        }

        // Draw the squares onto the image
        canvas.Draw(image);
        image.Format = MagickFormat.Png;
        return image;
    }

    public static bool IsGplColorLine(string line) =>
            line != GplHeader &&
            !line.StartsWith(GplName) &&
            !line.StartsWith(GplColumns) &&
            !line.StartsWith("#") &&
            !string.IsNullOrEmpty(line);

    public static IMagickColor<byte>? ParseColorGpl(string gplLine)
    {
        IMagickColor<byte>? result = null;

        List<string> rgb = [.. gplLine.Split(" ", StringSplitOptions.RemoveEmptyEntries).Take(3)];

        if (rgb.Count != 3)
        {
            return result;
        }

        if (byte.TryParse(rgb[0], out byte r) && 
            byte.TryParse(rgb[1], out byte g) && 
            byte.TryParse(rgb[2], out byte b))
        {
            result = new MagickColor(r, g, b);
        }

        return result;
    }

    public static List<IMagickColor<byte>> FromGpl(IEnumerable<string> lines)
    {
        List<IMagickColor<byte>> palette = [];

        bool valid = false;
        foreach(string line in lines)
        {
            if (!valid)
            {
                if (line == GplHeader)
                {
                    valid = true;
                }
                else
                {
                    return [];
                }
            }
            else if (IsGplColorLine(line))
            {
                IMagickColor<byte>? color = ParseColorGpl(line);
                if (color is null)
                {
                    return [];
                }

                palette.Add(color);
            }
        }

        return palette;
    }
}
