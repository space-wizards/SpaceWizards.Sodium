using System;
using System.Text;
using NUnit.Framework;

namespace SpaceWizards.Sodium.Tests;

[TestFixture]
public class PwHashTest
{
    [Test]
    public unsafe void Test()
    {
        const string password = "Correct Horse Battery Staple";
        const string expectedString = "DA8885E9B042F702B21812FB9A79C73F92C39C5FA1B9B5F95820C3C1B98A3850";
        var expected = Convert.FromHexString(expectedString);

        const string saltString = "D32A7FAF80A45FFACC900CA2AE0D4BE5";
        ReadOnlySpan<byte> saltSpan = new Span<byte>(Convert.FromHexString(saltString));
        Span<byte> key = stackalloc byte[32];

        var success = CryptoPwHash.Derive(key, saltSpan, Encoding.UTF8.GetBytes(password));

        Assert.That(success, Is.True);
        Assert.That(key.ToArray(), Is.EquivalentTo(expected));
    }
}
