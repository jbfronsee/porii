
using Conversion = Lib.Conversion;
using SimpleColor = Lib.SimpleColor;

using Wacton.Unicolour;

namespace Tests.App.Integration.Colors;

[TestClass]
public sealed class TestRgbHsvMap
{
    private SimpleColor.Rgb PackAndUnpack(SimpleColor.Rgb color)
    {
        // Pack Colors into ushort struct
        var (h, s, b) = new Unicolour(ColourSpace.Rgb255, color.R, color.G, color.B).Hsb;
        SimpleColor.PackedHsv packed = Conversion.Hsv.Pack(h / 360, s, b);

        // Unpack the same color and see what we obtained
        SimpleColor.Hsv simpleHsv = Conversion.Hsv.Unpack(packed);
        var back = new Unicolour(ColourSpace.Hsb, simpleHsv.H * 360, simpleHsv.S, simpleHsv.V).Rgb.Byte255;
        return new((byte)back.R, (byte)back.G, (byte)back.B);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public void Test_Full_RGB_Mapped()
    {
        TestRgbMap.Test_Full_RGB_Mapped(PackAndUnpack);
    }
}
