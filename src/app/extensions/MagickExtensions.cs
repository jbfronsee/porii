using ImageMagick;

public static class MagickExtensions
{
    /// <summary>
    /// Gets pixels as ByteColor.Rgb.
    /// </summary>
    /// <param name="image">The image to grab pixels from.</param>
    /// <returns></returns>
    /// <exception cref="MagickImageErrorException">Throws when library has an error or when channels < 3</exception>
    public static IEnumerable<SimpleColor.Rgb> GetPixelColors(this MagickImage image)
    {
        int channels = (int)image.ChannelCount;
        if (channels < 3)
        {
            throw new MagickImageErrorException("GetPixelColors() requires RGB channels to be present.");
        }

        IPixelCollection<byte> pixels = image.GetPixels();
        for (int y = 0; y < image.Height; y++)
        {
            byte[] pixelBytes = pixels.GetReadOnlyArea(0, y, image.Width, 1).ToArray();
            for (int x = 0; x < pixelBytes.Length; x += channels)
            {
                yield return new SimpleColor.Rgb(pixelBytes[x], pixelBytes[x + 1], pixelBytes[x + 2]);
            }
        }
    }
}