using App.Io;
using ImageMagick;

namespace Tests.App.Integration.Colors;

[TestClass]
public sealed class TestFormatGpl
{
    [TestMethod]
    [TestCategory("IntegrationTest")]
    [TestCategory("FastIntegration")]
    public void Test_Parse_Color_Gpl()
    {
        (byte, byte, byte) expected = (238, 242, 13);

        IMagickColor<byte>? color = Format.ParseColorGpl("238 242 13 Yellow");

        var actual = (color?.R, color?.G, color?.B);

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    [TestCategory("FastIntegration")]
    public void Test_Parse_Color_Gpl_Invalid()
    {
        IMagickColor<byte>? color = Format.ParseColorGpl("238 242");

        Assert.IsNull(color);
    }
}
