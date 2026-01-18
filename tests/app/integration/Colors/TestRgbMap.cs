namespace Tests.App.Integration.Colors;

public sealed class TestRgbMap
{
    public static void Test_Full_RGB_Mapped(Func<ColorRgb, ColorRgb> packAndUnpack)
    {
        // Expect to be able to get back every value that can be mapped in RGB
        int expected = (byte.MaxValue + 1) * (byte.MaxValue + 1) * (byte.MaxValue + 1);

        // Iterate through entire RGB space
        HashSet<ColorRgb> vals = [];
        for (int r = 0; r <= byte.MaxValue; r++)
        {
            for (int g = 0; g <= byte.MaxValue; g++)
            {
                for (int b = 0; b <= byte.MaxValue; b++)
                {
                    ColorRgb newVal = packAndUnpack(new ColorRgb((byte)r, (byte)g, (byte)b));

                    vals.Add(newVal);
                }
            }
        }

        Assert.HasCount(expected, vals);
    }
}
