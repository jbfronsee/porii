
using Wacton.Unicolour;

using Lib.Colors;

namespace Tests.App.Integration.Colors;

[TestClass]
public sealed class TestRgbLabMap
{
    private ColorRgb PackAndUnpack(ColorRgb color)
    {
        // Pack Colors into ushort struct
        var (uL, uA, uB) = new Unicolour(ColourSpace.Rgb255, color.R, color.G, color.B).Lab;
        PackedLab packed = new VectorLab(uL, uA, uB).Pack();

        // Unpack the same color and see what we obtained
        VectorLab lab = packed.Unpack();
        var back = new Unicolour(ColourSpace.Lab, lab.L, lab.A, lab.B).Rgb.Byte255;
        return new((byte)back.R, (byte)back.G, (byte)back.B);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public void Test_Full_RGB_Mapped()
    {
        TestRgbMap.Test_Full_RGB_Mapped(PackAndUnpack);
    }
}
