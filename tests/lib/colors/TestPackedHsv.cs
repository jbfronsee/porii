namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestPackedHsv
{
    // Some loss of precision is expected when converting back from ushort
    // 1e-5 can provide about 100,000 values between 0 and 1 which is what the HSV
    // Is converted from.
    private const double LargerEpsilon = 1e-5;

    private void Test_Pack_Hsv_Min_Logic(Func<ColorHsv, PackedHsv> packFunc)
    {
        ColorHsv value = new(0 - SharedConstants.Epsilon, 0 - SharedConstants.Epsilon, 0 - SharedConstants.Epsilon);
        PackedHsv expected = new(0, 0, 0);

        PackedHsv actual = packFunc(value);
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Min()
    {
        Test_Pack_Hsv_Min_Logic(PackedHsv.Pack);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Min_Doubles()
    {
        Test_Pack_Hsv_Min_Logic(c => c.Pack());
    }

    private void Test_Pack_Hsv_Max_Logic(Func<ColorHsv, PackedHsv> packFunc)
    {
        ColorHsv value = new(1 + SharedConstants.Epsilon, 1 + SharedConstants.Epsilon, 1 + SharedConstants.Epsilon);
        PackedHsv expected = new(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);
        
        PackedHsv actual = packFunc(value);
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Max()
    {
        Test_Pack_Hsv_Max_Logic(PackedHsv.Pack);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Max_Doubles()
    {
        Test_Pack_Hsv_Max_Logic(c => c.Pack());
    }

    private void Test_Pack_Hsv_Middle_Logic(Func<ColorHsv, PackedHsv> packFunc)
    {
        ColorHsv value = new(.5, .5, .5);
        
        PackedHsv actual = packFunc(value);
        
        Assert.IsInRange(32767, 32768, actual.H);
        Assert.IsInRange(32767, 32768, actual.S);
        Assert.IsInRange(32767, 32768, actual.V);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Middle()
    {
        Test_Pack_Hsv_Middle_Logic(PackedHsv.Pack);
    }

    [TestMethod]
    public void Test_Pack_Hsv_Middle_Doubles()
    {
        Test_Pack_Hsv_Middle_Logic(c => c.Pack());
    }

    private void Test_Unpack_Hsv_Min_Logic(Func<PackedHsv, ColorHsv> unpackFunc)
    {
        PackedHsv value = new(0, 0, 0);
        ColorHsv expected = new(0, 0, 0);

        ColorHsv actual = unpackFunc(value);
        
        Assert.AreEqual(expected.H, actual.H, SharedConstants.Epsilon);
        Assert.AreEqual(expected.S, actual.S, SharedConstants.Epsilon);
        Assert.AreEqual(expected.V, actual.V, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Min()
    {
        Test_Unpack_Hsv_Min_Logic(PackedHsv.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Min_Doubles()
    {
        Test_Unpack_Hsv_Min_Logic(c => c.Unpack());
    }

    private void Test_Unpack_Hsv_Max_Logic(Func<PackedHsv, ColorHsv> unpackFunc)
    {
        PackedHsv value = new(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);
        ColorHsv expected = new(1, 1, 1);
       
        
        ColorHsv actual = unpackFunc(value);
        
        Assert.AreEqual(expected.H, actual.H, SharedConstants.Epsilon);
        Assert.AreEqual(expected.S, actual.S, SharedConstants.Epsilon);
        Assert.AreEqual(expected.V, actual.V, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Max()
    {
        Test_Unpack_Hsv_Max_Logic(PackedHsv.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Max_Doubles()
    {
        Test_Unpack_Hsv_Max_Logic(c => c.Unpack());
    }

    private void Test_Unpack_Hsv_Middle_Logic(Func<PackedHsv, ColorHsv> unpackFunc)
    {
        PackedHsv value = new(32767, 32767, 32767);
        ColorHsv expected = new(.5, .5, .5);
        
        ColorHsv actual = unpackFunc(value);
        
        Assert.AreEqual(expected.H, actual.H, LargerEpsilon);
        Assert.AreEqual(expected.S, actual.S, LargerEpsilon);
        Assert.AreEqual(expected.V, actual.V, LargerEpsilon);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Middle()
    {
        Test_Unpack_Hsv_Middle_Logic(PackedHsv.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Hsv_Middle_Doubles()
    {
        Test_Unpack_Hsv_Middle_Logic(c => c.Unpack());
    }
}
