using ImageMagick;

public static class MagickExtensions
{
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