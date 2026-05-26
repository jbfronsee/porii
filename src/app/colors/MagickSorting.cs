using ImageMagick;

using Lib.Colors;

namespace App.Colors;

public static class MagickSorting
{
    /// <summary>
    /// Sorts enumerable by Hsv values and converts to a list of MagickColors.
    /// </summary>
    /// <param name="colors">input colors</param>
    /// <returns>New list of sorted colors</returns>
    public static List<IMagickColor<byte>> SortByHsv(IEnumerable<ColorRgb> colors)
    {
        return Sort(colors.Select(Colors.Convert.ToHsv));
    }

    /// <summary>
    /// Sorts enumerable by Hsv values and converts to a list of MagickColors.
    /// </summary>
    /// <param name="colors">input colors</param>
    /// <returns>New list of sorted colors</returns>
    public static List<IMagickColor<byte>> SortByHsv(IEnumerable<VectorLab> colors)
    {
        return Sort(colors.Select(Colors.Convert.ToHsv));
    }

    /// <summary>
    /// Sorts enumerable by Hsv values and converts to a list of MagickColors.
    /// </summary>
    /// <param name="colors">input colors</param>
    /// <returns>New list of sorted colors</returns>
    public static List<IMagickColor<byte>> Sort(IEnumerable<ColorHsv> colors)
    {
        return [.. colors.Order().Select(Colors.Convert.ToMagickColor)];
    }
}
