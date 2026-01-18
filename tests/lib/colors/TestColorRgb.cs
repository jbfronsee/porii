using Lib.Colors;

namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestColorRgb
{
    [TestMethod]
    [DataRow((byte)0, (byte)0, 0)]
    [DataRow((byte)0, (byte)1, -1)]
    [DataRow((byte)1, (byte)0, 1)]
    public void Test_RgbCompare_R(byte r, byte r2, int expected)
    {
        ColorRgb rgb = new(r, 0, 0);
        ColorRgb rgb2 = new(r2, 0, 0);

        int actual = rgb.CompareTo(rgb2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow((byte)0, (byte)0, 0)]
    [DataRow((byte)0, (byte)1, -1)]
    [DataRow((byte)1, (byte)0, 1)]
    public void Test_RgbCompare_G(byte g, byte g2, int expected)
    {
        ColorRgb rgb = new(0, g, 0);
        ColorRgb rgb2 = new(0, g2, 0);

        int actual = rgb.CompareTo(rgb2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow((byte)0, (byte)0, 0)]
    [DataRow((byte)0, (byte)1, -1)]
    [DataRow((byte)1, (byte)0, 1)]
    public void Test_RgbCompare_B(byte b, byte b2, int expected)
    {
        ColorRgb rgb = new(0, 0, b);
        ColorRgb rgb2 = new(0, 0, b2);

        int actual = rgb.CompareTo(rgb2);
        Assert.AreEqual(expected, actual);
    }
}
