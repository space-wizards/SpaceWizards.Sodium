namespace SpaceWizards.Sodium;

using static Interop.Libsodium;

/// <summary>
/// Wrappers around the <c>crypto_box</c> APIs.
/// </summary>
public static class CryptoBox
{
    static CryptoBox()
    {
        SodiumCore.EnsureInit();
    }

    public const int SealBytes = (int)crypto_box_SEALBYTES;
    public const int PublicKeyBytes = (int)crypto_box_PUBLICKEYBYTES;
    public const int SecretKeyBytes = (int)crypto_box_SECRETKEYBYTES;

    public static unsafe bool KeyPair(Span<byte> publicKey, Span<byte> secretKey)
    {
        if (publicKey.Length != PublicKeyBytes)
            throw new ArgumentException($"Public key must be {nameof(PublicKeyBytes)} bytes.");

        if (secretKey.Length != SecretKeyBytes)
            throw new ArgumentException($"Secret key must be {nameof(SecretKeyBytes)} bytes.");

        fixed (byte* pk = publicKey)
        fixed (byte* sk = secretKey)
        {
            return crypto_box_keypair(pk, sk) == 0;
        }
    }

    public static unsafe bool Seal(Span<byte> cipher, ReadOnlySpan<byte> message, ReadOnlySpan<byte> publicKey)
    {
        if (cipher.Length < checked(message.Length + SealBytes))
            throw new ArgumentException("Destination is too short");

        fixed (byte* c = cipher)
        fixed (byte* m = message)
        fixed (byte* pk = publicKey)
        {
            return crypto_box_seal(c, m, (ulong)message.Length, pk) == 0;
        }
    }

    public static byte[] Seal(ReadOnlySpan<byte> message, ReadOnlySpan<byte> publicKey)
    {
        var cipher = new byte[message.Length + SealBytes];
        if (!Seal(cipher, message, publicKey))
            throw new SodiumException("Seal failed");

        return cipher;
    }

    public static unsafe bool SealOpen(
        Span<byte> message,
        ReadOnlySpan<byte> cipher,
        ReadOnlySpan<byte> publicKey,
        ReadOnlySpan<byte> secretKey)
    {
        if (cipher.Length < SealBytes)
            throw new ArgumentException("Input is too short");

        if (message.Length < (cipher.Length - SealBytes))
            throw new ArgumentException("Destination is too short");

        if (publicKey.Length != PublicKeyBytes)
            throw new ArgumentException($"Public key must be {nameof(PublicKeyBytes)} bytes.");

        if (secretKey.Length != SecretKeyBytes)
            throw new ArgumentException($"Secret key must be {nameof(SecretKeyBytes)} bytes.");

        fixed (byte* m = message)
        fixed (byte* c = cipher)
        fixed (byte* pk = publicKey)
        fixed (byte* sk = secretKey)
        {
            return crypto_box_seal_open(m, c, (ulong)cipher.Length, pk, sk) == 0;
        }
    }

    public static byte[] SealOpen(ReadOnlySpan<byte> cipher, ReadOnlySpan<byte> publicKey, ReadOnlySpan<byte> secretKey)
    {
        if (cipher.Length < SealBytes)
            throw new ArgumentException("Input is too short");

        var message = new byte[cipher.Length - SealBytes];
        if (!SealOpen(message, cipher, publicKey, secretKey))
            throw new SodiumException("SealOpen failed");

        return message;
    }
}
