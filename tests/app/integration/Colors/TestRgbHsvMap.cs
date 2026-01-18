
using Wacton.Unicolour;

namespace Tests.App.Integration.Colors;

[TestClass]
public sealed class TestRgbHsvMap
{
    private ColorRgb PackAndUnpack(ColorRgb color)
    {
        // Pack Colors into ushort struct
        var (h, s, b) = new Unicolour(ColourSpace.Rgb255, color.R, color.G, color.B).Hsb;
        PackedHsv packed = new ColorHsv(h / 360, s, b).Pack();

        // Unpack the same color and see what we obtained
        ColorHsv hsv = packed.Unpack();
        var back = new Unicolour(ColourSpace.Hsb, hsv.H * 360, hsv.S, hsv.V).Rgb.Byte255;
        return new((byte)back.R, (byte)back.G, (byte)back.B);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    public void Test_Full_RGB_Mapped()
    {
        TestRgbMap.Test_Full_RGB_Mapped(PackAndUnpack);
    }
}
