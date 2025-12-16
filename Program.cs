using ImageMagick;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Processing Image...");

        var imageFromFile = new MagickImage("test.png");

        var hist = imageFromFile.Histogram();

        foreach (var color in hist)
        {
            Console.WriteLine($"Color: {color.Key} Occurences: {color.Value}");
        }
    }
}