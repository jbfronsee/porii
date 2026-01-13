
using Lib.Conversion;
using SimpleColor = Lib.SimpleColor;

namespace Tests.Lib.Conversion;

[TestClass]
public sealed class TestLab
{
    // Some loss of precision is expected when converting back from ushort
    
    // 1e-3 Can provide about 100,000 values between 0 and 100 for L.
    private const double LargerEpsilonL = 1e-3;

    // .002 Can provide close to 90 or 100k values between the AB bounds.
    private const double LargerEpsilonAB = .002;

    [TestMethod]
    public void Test_Pack_Lab_Min()
    {
        // Calculated Min Values
        SimpleColor.Lab value = new
        (
            Lab.LMin - SharedConstants.Epsilon,
            Lab.AMin - SharedConstants.Epsilon,
            Lab.BMin - SharedConstants.Epsilon
        );
        SimpleColor.PackedLab expected = new(0, 0, 0);

        SimpleColor.PackedLab actual = Lab.Pack(value);
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Pack_Lab_Max()
    {
        // Calculated Max Values
        SimpleColor.Lab value = new
        (
            Lab.LMax + SharedConstants.Epsilon,
            Lab.AMax + SharedConstants.Epsilon,
            Lab.BMax + SharedConstants.Epsilon
        );
        SimpleColor.PackedLab expected = new(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);

        SimpleColor.PackedLab actual = Lab.Pack(value);
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Pack_Lab_Middle()
    {
        // Values just have to be between max and min L a b scale = [0, 100] ~[-86, 98] ~[-107, 94]
        SimpleColor.Lab value = new(50, 5, 5);

        SimpleColor.PackedLab actual = Lab.Pack(value);
        
        Assert.IsInRange(32767, 32768, actual.L);
        Assert.IsInRange(32402, 32403, actual.A);
        Assert.IsInRange(36554, 36555, actual.B);
    }

    private void Test_Unpack_Lab_Min_Logic(Func<SimpleColor.PackedLab, SimpleColor.Lab> unpackFunc)
    {
        SimpleColor.PackedLab value = new(0, 0, 0);
        SimpleColor.Lab expected = new
        (
            Lab.LMin,
            Lab.AMin,
            Lab.BMin
        );

        SimpleColor.Lab actual = unpackFunc(value);
        
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Min()
    {
        Test_Unpack_Lab_Min_Logic(Lab.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Min_Doubles()
    {
        Test_Unpack_Lab_Min_Logic(c => Lab.Unpack(c.L, c.A, c.B));
    }

    private void Test_Unpack_Lab_Max_Logic(Func<SimpleColor.PackedLab, SimpleColor.Lab> unpackFunc)
    {
        // Calculated Max Values
        SimpleColor.PackedLab value = new(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);
        SimpleColor.Lab expected = new
        (
            Lab.LMax,
            Lab.AMax,
            Lab.BMax
        );
        
        SimpleColor.Lab actual = unpackFunc(value);
        
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Max()
    {
        Test_Unpack_Lab_Max_Logic(Lab.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Max_Doubles()
    {
        Test_Unpack_Lab_Max_Logic(c => Lab.Unpack(c.L, c.A, c.B));
    }

    private void Test_Unpack_Lab_Middle_Logic(Func<SimpleColor.PackedLab, SimpleColor.Lab> unpackFunc)
    {
        // Values just have to be between max and min L a b scale = [0, 100] ~[-86, 98] ~[-107, 94]
        SimpleColor.PackedLab value = new(32767, 32403, 36554);
        SimpleColor.Lab expected = new(50, 5, 5);
        
        SimpleColor.Lab actual = unpackFunc(value);
        
        Assert.AreEqual(expected.L, actual.L, LargerEpsilonL);
        Assert.AreEqual(expected.A, actual.A, LargerEpsilonAB);
        Assert.AreEqual(expected.B, actual.B, LargerEpsilonAB);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Middle()
    {
        Test_Unpack_Lab_Middle_Logic(Lab.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Middle_Doubles()
    {
        Test_Unpack_Lab_Middle_Logic(c => Lab.Unpack(c.L, c.A, c.B));
    }
}
