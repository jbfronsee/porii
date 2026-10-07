using App.Io;
using ImageMagick;

namespace Tests.App.Integration.Colors;

[TestClass]
public sealed class TestFormatGpl
{
    private static readonly string[] ValidGplLines = [
        "GIMP Palette",
        "Name: test",
        "Columns: 8",
        "#",
        "188 124  93     #0",
        "102  61  41     #1",
        " 45  22  11     #2"
    ];

    private static readonly string[] InvalidGplLinesNoHeader = [
        "Name: test",
        "Columns: 8",
        "#",
        "188 124  93     #0",
        "102  61  41     #1",
        " 45  22  11     #2"
    ];

    private static readonly string[] InvalidGplLinesNotByte = [
        "GIMP Palette",
        "Name: test",
        "Columns: 8",
        "#",
        "300 124  93     #0",
        "102  61  41     #1",
        " 45  22  11     #2"
    ];

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

    [TestMethod]
    [TestCategory("IntegrationTest")]
    [TestCategory("FastIntegration")]
    public void Test_From_Gpl_Valid()
    {
        List<(byte, byte, byte)> expected = [(188, 124, 93), (102, 61, 41), (45, 22, 11)];
        
        List<(byte, byte, byte)> actual = [.. Format.FromGpl(ValidGplLines).Select(c => (c.R, c.G, c.B))];

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    [TestCategory("FastIntegration")]
    public void Test_From_Gpl_Invalid_No_Header()
    {
        List<(byte, byte, byte)> expected = [];
        
        List<(byte, byte, byte)> actual = [.. Format.FromGpl(InvalidGplLinesNoHeader).Select(c => (c.R, c.G, c.B))];

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    [TestCategory("IntegrationTest")]
    [TestCategory("FastIntegration")]
    public void Test_From_Gpl_Invalid_Not_Byte()
    {
        List<(byte, byte, byte)> expected = [];
        
        List<(byte, byte, byte)> actual = [.. Format.FromGpl(InvalidGplLinesNotByte).Select(c => (c.R, c.G, c.B))];

        CollectionAssert.AreEqual(expected, actual);
    }
}
