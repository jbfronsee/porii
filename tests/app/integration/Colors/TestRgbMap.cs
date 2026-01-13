using SimpleColor = Lib.SimpleColor;

namespace Tests.App.Integration.Colors;

public sealed class TestRgbMap
{
    public static void Test_Full_RGB_Mapped(Func<SimpleColor.Rgb, SimpleColor.Rgb> packAndUnpack)
    {
        // Expect to be able to get back every value that can be mapped in RGB
        int expected = (byte.MaxValue + 1) * (byte.MaxValue + 1) * (byte.MaxValue + 1);

        // Iterate through entire RGB space
        HashSet<SimpleColor.Rgb> vals = [];
        for (int r = 0; r <= byte.MaxValue; r++)
        {
            for (int g = 0; g <= byte.MaxValue; g++)
            {
                for (int b = 0; b <= byte.MaxValue; b++)
                {
                    SimpleColor.Rgb newVal = packAndUnpack(new SimpleColor.Rgb((byte)r, (byte)g, (byte)b));

                    vals.Add(newVal);
                }
            }
        }

        Assert.HasCount(expected, vals);
    }
}
