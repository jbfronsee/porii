using ImageMagick;
using ImageMagick.Drawing;

public class Format
{
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
            "GIMP Palette",
            $"Name: {name}",
            $"Columns: {palette.Count}",
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
}