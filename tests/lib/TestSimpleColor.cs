using Lib.SimpleColor;

namespace Tests.Lib;

[TestClass]
public sealed class TestSimpleColor
{
    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_HsvCompare_Hue(double hue, double hue2, int expected)
    {
        HsvComparer comp = new();
        Hsv hsv = new(hue, 0, 0);
        Hsv hsv2 = new(hue2, 0, 0);

        int actual = comp.Compare(hsv, hsv2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_HsvCompare_Saturation(double saturation, double saturation2, int expected)
    {
        HsvComparer comp = new();
        Hsv hsv = new(0, saturation, 0);
        Hsv hsv2 = new(0, saturation2, 0);

        int actual = comp.Compare(hsv, hsv2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_HsvCompare_Value(double value, double value2, int expected)
    {
        HsvComparer comp = new();
        Hsv hsv = new(0, 0, value);
        Hsv hsv2 = new(0, 0, value2);

        int actual = comp.Compare(hsv, hsv2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_LabCompare_L(double l, double l2, int expected)
    {
        LabComparer comp = new();
        Lab lab = new(l, 0, 0);
        Lab lab2 = new(l2, 0, 0);

        int actual = comp.Compare(lab, lab2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_LabCompare_A(double a, double a2, int expected)
    {
        LabComparer comp = new();
        Lab lab = new(0, a, 0);
        Lab lab2 = new(0, a2, 0);

        int actual = comp.Compare(lab, lab2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_LabCompare_B(double b, double b2, int expected)
    {
        LabComparer comp = new();
        Lab lab = new(0, 0, b);
        Lab lab2 = new(0, 0, b2);

        int actual = comp.Compare(lab, lab2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow((byte)0, (byte)0, 0)]
    [DataRow((byte)0, (byte)1, -1)]
    [DataRow((byte)1, (byte)0, 1)]
    public void Test_RgbCompare_R(byte r, byte r2, int expected)
    {
        RgbComparer comp = new();
        Rgb rgb = new(r, 0, 0);
        Rgb rgb2 = new(r2, 0, 0);

        int actual = comp.Compare(rgb, rgb2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow((byte)0, (byte)0, 0)]
    [DataRow((byte)0, (byte)1, -1)]
    [DataRow((byte)1, (byte)0, 1)]
    public void Test_HsvCompare_G(byte g, byte g2, int expected)
    {
        RgbComparer comp = new();
        Rgb rgb = new(0, g, 0);
        Rgb rgb2 = new(0, g2, 0);

        int actual = comp.Compare(rgb, rgb2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow((byte)0, (byte)0, 0)]
    [DataRow((byte)0, (byte)1, -1)]
    [DataRow((byte)1, (byte)0, 1)]
    public void Test_HsvCompare_B(byte b, byte b2, int expected)
    {
        RgbComparer comp = new();
        Rgb rgb = new(0, 0, b);
        Rgb rgb2 = new(0, 0, b2);

        int actual = comp.Compare(rgb, rgb2);
        Assert.AreEqual(expected, actual);
    }
}
