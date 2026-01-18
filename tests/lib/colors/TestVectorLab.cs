using Lib.Colors;

namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestVectorLab
{
    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_LabCompare_L(double l, double l2, int expected)
    {
        VectorLab lab = new(l, 0, 0);
        VectorLab lab2 = new(l2, 0, 0);

        int actual = lab.CompareTo(lab2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_LabCompare_A(double a, double a2, int expected)
    {
        VectorLab lab = new(0, a, 0);
        VectorLab lab2 = new(0, a2, 0);

        int actual = lab.CompareTo(lab2);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(0, 0, 0)]
    [DataRow(0, 1, -1)]
    [DataRow(1, 0, 1)]
    public void Test_LabCompare_B(double b, double b2, int expected)
    {
        VectorLab lab = new(0, 0, b);
        VectorLab lab2 = new(0, 0, b2);

        int actual = lab.CompareTo(lab2);
        Assert.AreEqual(expected, actual);
    }
}
