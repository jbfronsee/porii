
using Lib.Conversion;
using SimpleColor = Lib.SimpleColor;

namespace Tests.Lib.Conversion;

[TestClass]
public sealed class TestHsv
{
    // Some loss of precision is expected when converting back from ushort
    // 1e-5 can provide about 100,000 values between 0 and 1 which is what the HSV
    // Is converted from.
    private const double LargerEpsilon = 1e-5;

    private void Test_Pack_Hsv_Min_Logic(Func<SimpleColor.Hsv, SimpleColor.PackedHsv> packFunc)
    {
        SimpleColor.Hsv value = new(0 - SharedConstants.Epsilon, 0 - SharedConstants.Epsilon, 0 - SharedConstants.Epsilon);
        SimpleColor.PackedHsv expected = new(0, 0, 0);

        SimpleColor.PackedHsv actual = packFunc(value);
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Min()
    {
        Test_Pack_Hsv_Min_Logic(Hsv.Pack);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Min_Doubles()
    {
        Test_Pack_Hsv_Min_Logic(c => Hsv.Pack(c.H, c.S, c.V));
    }

    private void Test_Pack_Hsv_Max_Logic(Func<SimpleColor.Hsv, SimpleColor.PackedHsv> packFunc)
    {
        SimpleColor.Hsv value = new(1 + SharedConstants.Epsilon, 1 + SharedConstants.Epsilon, 1 + SharedConstants.Epsilon);
        SimpleColor.PackedHsv expected = new(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);
        
        SimpleColor.PackedHsv actual = packFunc(value);
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Max()
    {
        Test_Pack_Hsv_Max_Logic(Hsv.Pack);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Max_Doubles()
    {
        Test_Pack_Hsv_Max_Logic(c => Hsv.Pack(c.H, c.S, c.V));
    }

    private void Test_Pack_Hsv_Middle_Logic(Func<SimpleColor.Hsv, SimpleColor.PackedHsv> packFunc)
    {
        SimpleColor.Hsv value = new(.5, .5, .5);
        
        SimpleColor.PackedHsv actual = packFunc(value);
        
        Assert.IsInRange(32767, 32768, actual.H);
        Assert.IsInRange(32767, 32768, actual.S);
        Assert.IsInRange(32767, 32768, actual.V);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Middle()
    {
        Test_Pack_Hsv_Middle_Logic(Hsv.Pack);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Middle_Doubles()
    {
        Test_Pack_Hsv_Middle_Logic(c => Hsv.Pack(c.H, c.S, c.V));
    }

    private void Test_Unpack_Hsv_Min_Logic(Func<SimpleColor.PackedHsv, SimpleColor.Hsv> unpackFunc)
    {
        SimpleColor.PackedHsv value = new(0, 0, 0);
        SimpleColor.Hsv expected = new(0, 0, 0);

        SimpleColor.Hsv actual = unpackFunc(value);
        
        Assert.AreEqual(expected.H, actual.H, SharedConstants.Epsilon);
        Assert.AreEqual(expected.S, actual.S, SharedConstants.Epsilon);
        Assert.AreEqual(expected.V, actual.V, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Min()
    {
        Test_Unpack_Hsv_Min_Logic(Hsv.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Min_Doubles()
    {
        Test_Unpack_Hsv_Min_Logic(c => Hsv.Unpack(c.H, c.S, c.V));
    }

    private void Test_Unpack_Hsv_Max_Logic(Func<SimpleColor.PackedHsv, SimpleColor.Hsv> unpackFunc)
    {
        SimpleColor.PackedHsv value = new(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);
        SimpleColor.Hsv expected = new(1, 1, 1);
       
        
        SimpleColor.Hsv actual = unpackFunc(value);
        
        Assert.AreEqual(expected.H, actual.H, SharedConstants.Epsilon);
        Assert.AreEqual(expected.S, actual.S, SharedConstants.Epsilon);
        Assert.AreEqual(expected.V, actual.V, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Max()
    {
        Test_Unpack_Hsv_Max_Logic(Hsv.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Max_Doubles()
    {
        Test_Unpack_Hsv_Max_Logic(c => Hsv.Unpack(c.H, c.S, c.V));
    }

    private void Test_Unpack_Hsv_Middle_Logic(Func<SimpleColor.PackedHsv, SimpleColor.Hsv> unpackFunc)
    {
        SimpleColor.PackedHsv value = new(32767, 32767, 32767);
        SimpleColor.Hsv expected = new(.5, .5, .5);
        
        SimpleColor.Hsv actual = unpackFunc(value);
        
        Assert.AreEqual(expected.H, actual.H, LargerEpsilon);
        Assert.AreEqual(expected.S, actual.S, LargerEpsilon);
        Assert.AreEqual(expected.V, actual.V, LargerEpsilon);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Middle()
    {
        Test_Unpack_Hsv_Middle_Logic(Hsv.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Middle_Doubles()
    {
        Test_Unpack_Hsv_Middle_Logic(c => Hsv.Unpack(c.H, c.S, c.V));
    }
}
