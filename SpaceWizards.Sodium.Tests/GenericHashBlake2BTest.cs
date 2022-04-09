using System;
using System.Text;
using NUnit.Framework;

namespace SpaceWizards.Sodium.Tests;

[TestFixture]
[TestOf(typeof(CryptoGenericHashBlake2B))]
[Parallelizable(ParallelScope.All)]
public sealed class GenericHashBlake2BTest
{
    [Test]
    [TestCase("foobar", null, "93a0e84a8cdd4166267dbe1263e937f08087723ac24e7dcc35b3d5941775ef47")]
    [TestCase("foobar", null,
        "8df31f60d6aeabd01b7dc83f277d0e24cbe104f7290ff89077a7eb58646068edfe1a83022866c46f65fb91612e516e0ecfa5cb25fc16b37d2c8d73732fe74cb2")]
    [TestCase("foobar", "baz", "5f49889d5da33b8a04b242f19986193f0401d8fe087040ed79ae955119638a45")]
    public void TestHash(string inputStr, string? keyStr, string expectedHex)
    {
        var input = Encoding.UTF8.GetBytes(inputStr);
        var key = keyStr == null ? null : Encoding.UTF8.GetBytes(keyStr);
        var expected = Convert.FromHexString(expectedHex);

        var output = CryptoGenericHashBlake2B.Hash(expected.Length, input, key);

        Assert.That(output, Is.EquivalentTo(expected));
    }

    [Test]
    // @formatter:off
    [TestCase("foobar", null, null, null, "93a0e84a8cdd4166267dbe1263e937f08087723ac24e7dcc35b3d5941775ef47")]
    [TestCase("foobar", null, null, null, "8df31f60d6aeabd01b7dc83f277d0e24cbe104f7290ff89077a7eb58646068edfe1a83022866c46f65fb91612e516e0ecfa5cb25fc16b37d2c8d73732fe74cb2")]
    [TestCase("foobar", "baz", null, null, "5f49889d5da33b8a04b242f19986193f0401d8fe087040ed79ae955119638a45")]
    [TestCase("foobar", "baz", "AAAABBBBCCCCDDDD", null, "d4898e5ec36873d27d87ab00a464be00a7c8be03b9b5c01defd6e9d1e7150ebb")]
    [TestCase("foobar", "baz", "AAAABBBBCCCCDDDD", "DDDDCCCCBBBBAAAA", "ccfa20580f3162c9312aa9ba39e88bc1e6857ebb5dcad2726d3835207cf8d735")]
    [TestCase("foobar", "baz", null, "DDDDCCCCBBBBAAAA", "75d3b6c777060f178299a5fb16846013fb97354305598636117493a57b117282")]
    [TestCase("foobar", null, null, "DDDDCCCCBBBBAAAA", "f5f01f9992ce6db03a37b9485d17bdc8f26154d6bc70e0524124b9bbeafd0269")]
    // @formatter:on
    public void TestHashSaltPersonal(
        string inputStr,
        string? keyStr,
        string? saltStr,
        string? personalStr,
        string expectedHex)
    {
        var input = Encoding.UTF8.GetBytes(inputStr);
        var key = keyStr == null ? null : Encoding.UTF8.GetBytes(keyStr);
        var salt = saltStr == null ? null : Encoding.UTF8.GetBytes(saltStr);
        var personal = personalStr == null ? null : Encoding.UTF8.GetBytes(personalStr);
        var expected = Convert.FromHexString(expectedHex);

        var output = CryptoGenericHashBlake2B.HashSaltPersonal(expected.Length, input, key, salt, personal);

        Assert.That(output, Is.EquivalentTo(expected));
    }

    [Test]
    [TestCase("foobar", null, "93a0e84a8cdd4166267dbe1263e937f08087723ac24e7dcc35b3d5941775ef47")]
    [TestCase("foobar", null,
        "8df31f60d6aeabd01b7dc83f277d0e24cbe104f7290ff89077a7eb58646068edfe1a83022866c46f65fb91612e516e0ecfa5cb25fc16b37d2c8d73732fe74cb2")]
    [TestCase("foobar", "baz", "5f49889d5da33b8a04b242f19986193f0401d8fe087040ed79ae955119638a45")]
    public void TestHashIncremental(string inputStr, string? keyStr, string expectedHex)
    {
        var input = Encoding.UTF8.GetBytes(inputStr);
        var key = keyStr == null ? null : Encoding.UTF8.GetBytes(keyStr);
        var expected = Convert.FromHexString(expectedHex);

        var output = new byte[expected.Length];
        CryptoGenericHashBlake2B.State state;
        CryptoGenericHashBlake2B.Init(ref state, key, expected.Length);
        CryptoGenericHashBlake2B.Update(ref state, input);
        CryptoGenericHashBlake2B.Final(ref state, output);

        Assert.That(output, Is.EquivalentTo(expected));
    }

    [Test]
    // @formatter:off
    [TestCase("foobar", null, null, null, "93a0e84a8cdd4166267dbe1263e937f08087723ac24e7dcc35b3d5941775ef47")]
    [TestCase("foobar", null, null, null, "8df31f60d6aeabd01b7dc83f277d0e24cbe104f7290ff89077a7eb58646068edfe1a83022866c46f65fb91612e516e0ecfa5cb25fc16b37d2c8d73732fe74cb2")]
    [TestCase("foobar", "baz", null, null, "5f49889d5da33b8a04b242f19986193f0401d8fe087040ed79ae955119638a45")]
    [TestCase("foobar", "baz", "AAAABBBBCCCCDDDD", null, "d4898e5ec36873d27d87ab00a464be00a7c8be03b9b5c01defd6e9d1e7150ebb")]
    [TestCase("foobar", "baz", "AAAABBBBCCCCDDDD", "DDDDCCCCBBBBAAAA", "ccfa20580f3162c9312aa9ba39e88bc1e6857ebb5dcad2726d3835207cf8d735")]
    [TestCase("foobar", "baz", null, "DDDDCCCCBBBBAAAA", "75d3b6c777060f178299a5fb16846013fb97354305598636117493a57b117282")]
    [TestCase("foobar", null, null, "DDDDCCCCBBBBAAAA", "f5f01f9992ce6db03a37b9485d17bdc8f26154d6bc70e0524124b9bbeafd0269")]
    // @formatter:on
    public void TestHashSaltPersonalIncremental(
        string inputStr,
        string? keyStr,
        string? saltStr,
        string? personalStr,
        string expectedHex)
    {
        var input = Encoding.UTF8.GetBytes(inputStr);
        var key = keyStr == null ? null : Encoding.UTF8.GetBytes(keyStr);
        var salt = saltStr == null ? null : Encoding.UTF8.GetBytes(saltStr);
        var personal = personalStr == null ? null : Encoding.UTF8.GetBytes(personalStr);
        var expected = Convert.FromHexString(expectedHex);

        var output = new byte[expected.Length];
        CryptoGenericHashBlake2B.State state;
        CryptoGenericHashBlake2B.InitSaltPersonal(ref state, key, expected.Length, salt, personal);
        CryptoGenericHashBlake2B.Update(ref state, input);
        CryptoGenericHashBlake2B.Final(ref state, output);

        Assert.That(output, Is.EquivalentTo(expected));
    }
}
