
using Conversion = Lib.Conversion;
using SimpleColor = Lib.SimpleColor;

using Wacton.Unicolour;

namespace Tests.App.Integration.Colors;

[TestClass]
public sealed class TestRgbLabMap
{
    private SimpleColor.Rgb PackAndUnpack(SimpleColor.Rgb color)
    {
        // Pack Colors into ushort struct
        var (uL, uA, uB) = new Unicolour(ColourSpace.Rgb255, color.R, color.G, color.B).Lab;
        SimpleColor.PackedLab packed = Conversion.Lab.Pack(uL, uA, uB);

        // Unpack the same color and see what we obtained
        SimpleColor.Lab simpleLab = Conversion.Lab.Unpack(packed);
        var back = new Unicolour(ColourSpace.Lab, simpleLab.L, simpleLab.A, simpleLab.B).Rgb.Byte255;
        return new((byte)back.R, (byte)back.G, (byte)back.B);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public void Test_Full_RGB_Mapped()
    {
        TestRgbMap.Test_Full_RGB_Mapped(PackAndUnpack);
    }
}
