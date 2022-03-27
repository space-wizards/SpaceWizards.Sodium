namespace SpaceWizards.Sodium;
using static Interop.Libsodium;

/// <summary>
/// Wrappers around the <c>crypto_aead_xchacha20poly1305_ietf_</c> APIs.
/// </summary>
public static class CryptoAeadXChaCha20Poly1305Ietf
{
    static CryptoAeadXChaCha20Poly1305Ietf()
    {
        SodiumCore.EnsureInit();
    }

    public const int NoncePublicBytes = (int)crypto_aead_xchacha20poly1305_ietf_NPUBBYTES;
    public const int KeyBytes = (int)crypto_aead_xchacha20poly1305_ietf_KEYBYTES;
    public const int AddBytes = (int)crypto_aead_xchacha20poly1305_ietf_ABYTES;

    public static unsafe void Keygen(Span<byte> key)
    {
        if (key.Length != KeyBytes)
            throw new ArgumentException($"Key must be {nameof(KeyBytes)} bytes");

        fixed (byte* k = key)
        {
            crypto_aead_xchacha20poly1305_ietf_keygen(k);
        }
    }

    public static byte[] Keygen()
    {
        var key = new byte[KeyBytes];
        Keygen(key);
        return key;
    }

    public static unsafe bool Encrypt(
        Span<byte> cipher,
        out int cipherLength,
        ReadOnlySpan<byte> message,
        ReadOnlySpan<byte> additionalData,
        ReadOnlySpan<byte> noncePublic,
        ReadOnlySpan<byte> key)
    {
        if (cipher.Length < checked(message.Length + AddBytes))
            throw new ArgumentException("Destination is too short");

        if (key.Length != KeyBytes)
            throw new ArgumentException($"Key must be {nameof(KeyBytes)} bytes");

        if (noncePublic.Length != NoncePublicBytes)
            throw new ArgumentException($"Nonce must be {nameof(NoncePublicBytes)} bytes");

        fixed (byte* c = cipher)
        fixed (byte* m = message)
        fixed (byte* ad = additionalData)
        fixed (byte* npub = noncePublic)
        fixed (byte* k = key)
        {
            ulong clen;
            var ret = crypto_aead_xchacha20poly1305_ietf_encrypt(
                c, &clen,
                m, (ulong)message.Length,
                ad, (ulong)additionalData.Length,
                null,
                npub,
                k);

            cipherLength = (int)clen;
            return ret == 0;
        }
    }

    public static unsafe bool Decrypt(
        Span<byte> message,
        out int messageLength,
        ReadOnlySpan<byte> cipher,
        ReadOnlySpan<byte> additionalData,
        ReadOnlySpan<byte> noncePublic,
        ReadOnlySpan<byte> key)
    {
        if (cipher.Length < AddBytes)
            throw new ArgumentException("Input is too short");

        if (message.Length < cipher.Length - AddBytes)
            throw new ArgumentException("Output is too short");

        if (key.Length != KeyBytes)
            throw new ArgumentException($"Key must be {nameof(KeyBytes)} bytes");

        if (noncePublic.Length != NoncePublicBytes)
            throw new ArgumentException($"Nonce must be {nameof(NoncePublicBytes)} bytes");

        fixed (byte* c = cipher)
        fixed (byte* m = message)
        fixed (byte* ad = additionalData)
        fixed (byte* npub = noncePublic)
        fixed (byte* k = key)
        {
            ulong mlen;
            var ret = crypto_aead_xchacha20poly1305_ietf_decrypt(
                m, &mlen,
                null,
                c, (ulong)cipher.Length,
                ad, (ulong)additionalData.Length,
                npub,
                k);

            messageLength = (int)mlen;
            return ret == 0;
        }
    }
}
