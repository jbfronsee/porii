using Lib.Colors;

namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestColorHsv
{
    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_HsvCompare_Hue(double hue, double hue2, int expected)
    {
        ColorHsv hsv = new(hue, 0, 0);
        ColorHsv hsv2 = new(hue2, 0, 0);

        int actual = hsv.CompareTo(hsv2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_HsvCompare_Saturation(double saturation, double saturation2, int expected)
    {
        ColorHsv hsv = new(0, saturation, 0);
        ColorHsv hsv2 = new(0, saturation2, 0);

        int actual = hsv.CompareTo(hsv2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_HsvCompare_Value(double value, double value2, int expected)
    {
        ColorHsv hsv = new(0, 0, value);
        ColorHsv hsv2 = new(0, 0, value2);

        int actual = hsv.CompareTo(hsv2);
        Assert.AreEqual(expected, actual);
    }
}
