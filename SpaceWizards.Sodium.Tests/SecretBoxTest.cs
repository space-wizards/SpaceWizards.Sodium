using System.Security.Cryptography;
using NUnit.Framework;
using static SpaceWizards.Sodium.Interop.Libsodium;

namespace SpaceWizards.Sodium.Tests;

[TestFixture]
public sealed class SecretBoxTest
{
    [Test]
    public unsafe void Test()
    {
        var message = RandomNumberGenerator.GetBytes(1024);
        var cipher = RandomNumberGenerator.GetBytes((int)(1024 + crypto_secretbox_MACBYTES));

        var key = stackalloc byte[(int)crypto_secretbox_KEYBYTES];
        var nonce = stackalloc byte[(int)crypto_secretbox_NONCEBYTES];

        crypto_secretbox_keygen(key);
        randombytes_buf(nonce, crypto_secretbox_NONCEBYTES);
        fixed (byte* mPtr = message)
        fixed (byte* cPtr = cipher)
        {
            crypto_secretbox_easy(cPtr, mPtr, (ulong)message.Length, nonce, key);
        }

        var decrypted = new byte[message.Length];
        fixed (byte* dPtr = decrypted)
        fixed (byte* cPtr = cipher)
        {
            crypto_secretbox_open_easy(dPtr, cPtr, (ulong)cipher.Length, nonce, key);
        }

        Assert.That(decrypted, Is.EquivalentTo(message));
    }
}
