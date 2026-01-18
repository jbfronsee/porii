using Lib.Colors;

namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestPackedLab
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
        VectorLab value = new
        (
            PackedLab.LMin - SharedConstants.Epsilon,
            PackedLab.AMin - SharedConstants.Epsilon,
            PackedLab.BMin - SharedConstants.Epsilon
        );
        PackedLab expected = new(0, 0, 0);

        PackedLab actual = PackedLab.Pack(value);
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Pack_Lab_Max()
    {
        // Calculated Max Values
        VectorLab value = new
        (
            PackedLab.LMax + SharedConstants.Epsilon,
            PackedLab.AMax + SharedConstants.Epsilon,
            PackedLab.BMax + SharedConstants.Epsilon
        );
        PackedLab expected = new(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);

        PackedLab actual = PackedLab.Pack(value);
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_Pack_Lab_Middle()
    {
        // Values just have to be between max and min L a b scale = [0, 100] ~[-86, 98] ~[-107, 94]
        VectorLab value = new(50, 5, 5);

        PackedLab actual = PackedLab.Pack(value);
        
        Assert.IsInRange(32767, 32768, actual.L);
        Assert.IsInRange(32402, 32403, actual.A);
        Assert.IsInRange(36554, 36555, actual.B);
    }

    private void Test_Unpack_Lab_Min_Logic(Func<PackedLab, VectorLab> unpackFunc)
    {
        PackedLab value = new(0, 0, 0);
        VectorLab expected = new
        (
            PackedLab.LMin,
            PackedLab.AMin,
            PackedLab.BMin
        );

        VectorLab actual = unpackFunc(value);
        
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Min()
    {
        Test_Unpack_Lab_Min_Logic(PackedLab.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Min_Doubles()
    {
        Test_Unpack_Lab_Min_Logic(c => c.Unpack());
    }

    private void Test_Unpack_Lab_Max_Logic(Func<PackedLab, VectorLab> unpackFunc)
    {
        // Calculated Max Values
        PackedLab value = new(ushort.MaxValue, ushort.MaxValue, ushort.MaxValue);
        VectorLab expected = new
        (
            PackedLab.LMax,
            PackedLab.AMax,
            PackedLab.BMax
        );
        
        VectorLab actual = unpackFunc(value);
        
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Max()
    {
        Test_Unpack_Lab_Max_Logic(PackedLab.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Max_Doubles()
    {
        Test_Unpack_Lab_Max_Logic(c => c.Unpack());
    }

    private void Test_Unpack_Lab_Middle_Logic(Func<PackedLab, VectorLab> unpackFunc)
    {
        // Values just have to be between max and min L a b scale = [0, 100] ~[-86, 98] ~[-107, 94]
        PackedLab value = new(32767, 32403, 36554);
        VectorLab expected = new(50, 5, 5);
        
        VectorLab actual = unpackFunc(value);
        
        Assert.AreEqual(expected.L, actual.L, LargerEpsilonL);
        Assert.AreEqual(expected.A, actual.A, LargerEpsilonAB);
        Assert.AreEqual(expected.B, actual.B, LargerEpsilonAB);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Middle()
    {
        Test_Unpack_Lab_Middle_Logic(PackedLab.Unpack);
    }

    [TestMethod]
    public void Test_Unpack_Lab_Middle_Doubles()
    {
        Test_Unpack_Lab_Middle_Logic(c => c.Unpack());
    }
}
