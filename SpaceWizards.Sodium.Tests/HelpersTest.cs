using System;
using System.Text;
using NUnit.Framework;

namespace SpaceWizards.Sodium.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[TestOf(typeof(SodiumHelpers))]
public class HelpersTest
{
    [Test]
    [TestCase("foobar", "foobar", ExpectedResult = true)]
    [TestCase("foobar", "foobaz", ExpectedResult = false)]
    public bool TestMemoryCompare(string b1Str, string b2Str)
    {
        var b1 = Encoding.UTF8.GetBytes(b1Str);
        var b2 = Encoding.UTF8.GetBytes(b2Str);

        return SodiumHelpers.MemoryCompare(b1, b2);
    }


    [Test]
    [TestCase("DEADBEEF", "deadbeef")]
    public void TestBin2Hex(string binHex, string expectHex)
    {
        var bin = Convert.FromHexString(binHex);

        var hex = SodiumHelpers.Bin2Hex(bin);

        Assert.That(hex, Is.EqualTo(expectHex).IgnoreCase);
    }

    [Test]
    [TestCase("F9FD8217F8F00EE5", SodiumBase64Variant.Original, ExpectedResult = "+f2CF/jwDuU=")]
    [TestCase("F9FD8217F8F00EE5", SodiumBase64Variant.OriginalNoPadding, ExpectedResult = "+f2CF/jwDuU")]
    [TestCase("F9FD8217F8F00EE5", SodiumBase64Variant.OriginalUrlSafe, ExpectedResult = "-f2CF_jwDuU=")]
    [TestCase("F9FD8217F8F00EE5", SodiumBase64Variant.OriginalUrlSafeNoPadding, ExpectedResult = "-f2CF_jwDuU")]
    public string TestBin2Base64(string binHex, SodiumBase64Variant variant)
    {
        var bin = Convert.FromHexString(binHex);

        return SodiumHelpers.Bin2Base64(bin, variant);
    }

    [Test]
    [TestCase("00000000000000000000000000000000", ExpectedResult = "01000000000000000000000000000000")]
    [TestCase("FF000000000000000000000000000000", ExpectedResult = "00010000000000000000000000000000")]
    [TestCase("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00", ExpectedResult = "00000000000000000000000000000001")]
    public string TestIncrement(string nHex)
    {
        var n = Convert.FromHexString(nHex);

        SodiumHelpers.Increment(n);

        return Convert.ToHexString(n);
    }

    [Test]
    // @formatter:off
    [TestCase("00000000000000000000000000000000", "00000000000000000000000000000000", ExpectedResult = "00000000000000000000000000000000")]
    [TestCase("FF000000000000000000000000000000", "FF000000000000000000000000000000", ExpectedResult = "FE010000000000000000000000000000")]
    [TestCase("576F809E99346A475F20727603A1D41C", "6AE9809052ACEA8F7535E6ADE2613B6F", ExpectedResult = "C158012FECE054D7D4555824E602108C")]
    // @formatter:on
    public string TestAdd(string aHex, string bHex)
    {
        var a = Convert.FromHexString(aHex);
        var b = Convert.FromHexString(bHex);

        SodiumHelpers.Add(a, b);

        return Convert.ToHexString(a);
    }

    [Test]
    // @formatter:off
    [TestCase("00000000000000000000000000000000", "00000000000000000000000000000000", ExpectedResult = "00000000000000000000000000000000")]
    [TestCase("FE010000000000000000000000000000", "FF000000000000000000000000000000", ExpectedResult = "FF000000000000000000000000000000")]
    [TestCase("6AE9809052ACEA8F7535E6ADE2613B6F", "576F809E99346A475F20727603A1D41C", ExpectedResult = "137A00F2B877804816157437DFC06652")]
    // @formatter:on
    public string TestSub(string aHex, string bHex)
    {
        var a = Convert.FromHexString(aHex);
        var b = Convert.FromHexString(bHex);

        SodiumHelpers.Sub(a, b);

        return Convert.ToHexString(a);
    }

    [Test]
    [TestCase("00000000000000000000000000000000", "00000000000000000000000000000000", ExpectedResult = 0)]
    [TestCase("FE010000000000000000000000000000", "FF000000000000000000000000000000", ExpectedResult = 1)]
    [TestCase("576F809E99346A475F20727603A1D41C", "6AE9809052ACEA8F7535E6ADE2613B6F", ExpectedResult = -1)]
    public int TestCompare(string aHex, string bHex)
    {
        var a = Convert.FromHexString(aHex);
        var b = Convert.FromHexString(bHex);

        return SodiumHelpers.Compare(a, b);
    }

    [Test]
    [TestCase("00000000000000000000000000000000", ExpectedResult = true)]
    [TestCase("FE010000000000000000000000000000", ExpectedResult = false)]
    public bool TestIsZero(string nHex)
    {
        var n = Convert.FromHexString(nHex);

        return SodiumHelpers.IsZero(n);
    }

    [Test]
    public void StackZero()
    {
        // I mean there's not a whole lot I can verify here.
        // I guess just test that it doesn't crash?

        SodiumHelpers.StackZero(32);
    }
}
