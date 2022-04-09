using SpaceWizards.Sodium.Interop;

namespace SpaceWizards.Sodium;

using static Interop.Libsodium;

/// <summary>
/// Wrappers around the <c>crypto_generichash_blake2b_</c> APIs.
/// </summary>
public static class CryptoGenericHashBlake2B
{
    static CryptoGenericHashBlake2B()
    {
        SodiumCore.EnsureInit();
    }

    public const int BytesMin = (int)crypto_generichash_blake2b_BYTES_MIN;
    public const int BytesMax = (int)crypto_generichash_blake2b_BYTES_MAX;
    public const int Bytes = (int)crypto_generichash_blake2b_BYTES;

    public const int KeyBytesMin = (int)crypto_generichash_blake2b_KEYBYTES_MIN;
    public const int KeyBytesMax = (int)crypto_generichash_blake2b_KEYBYTES_MAX;
    public const int KeyBytes = (int)crypto_generichash_blake2b_BYTES;

    public const int SaltBytes = (int)crypto_generichash_blake2b_SALTBYTES;
    public const int PersonalBytes = (int)crypto_generichash_blake2b_PERSONALBYTES;

    public static unsafe void Keygen(Span<byte> key)
    {
        if (key.Length != KeyBytes)
            throw new ArgumentException($"Key must be {nameof(KeyBytes)} bytes");

        fixed (byte* k = key)
        {
            crypto_generichash_blake2b_keygen(k);
        }
    }

    public static byte[] Keygen()
    {
        var key = new byte[KeyBytes];
        Keygen(key);
        return key;
    }

    public static byte[] Hash(
        int outputLength,
        ReadOnlySpan<byte> input,
        ReadOnlySpan<byte> key)
    {
        var output = new byte[outputLength];

        Hash(output, input, key);

        return output;
    }

    public static unsafe bool Hash(Span<byte> output, ReadOnlySpan<byte> input, ReadOnlySpan<byte> key)
    {
        if (key.Length > KeyBytesMax)
            throw new ArgumentException("Key too large");

        if (output.Length is < BytesMin or > BytesMax)
            throw new ArgumentException("Output is invalid size");

        fixed (byte* i = input)
        fixed (byte* o = output)
        fixed (byte* k = key)
        {
            var ret = crypto_generichash_blake2b(
                o, (nuint)output.Length,
                i, (ulong)input.Length,
                k, (nuint)key.Length);

            return ret == 0;
        }
    }

    public static byte[] HashSaltPersonal(
        int outputLength,
        ReadOnlySpan<byte> input,
        ReadOnlySpan<byte> key,
        ReadOnlySpan<byte> salt,
        ReadOnlySpan<byte> personal)
    {
        var output = new byte[outputLength];

        HashSaltPersonal(output, input, key, salt, personal);

        return output;
    }


    public static unsafe bool HashSaltPersonal(
        Span<byte> output,
        ReadOnlySpan<byte> input,
        ReadOnlySpan<byte> key,
        ReadOnlySpan<byte> salt,
        ReadOnlySpan<byte> personal)
    {
        if (key.Length > KeyBytesMax)
            throw new ArgumentException("Key too large");

        if (output.Length is < BytesMin or > BytesMax)
            throw new ArgumentException("Output is invalid size");

        if (salt.Length != SaltBytes && salt.Length != 0)
            throw new ArgumentException($"Salt must be {nameof(SaltBytes)} bytes or empty");

        if (personal.Length != PersonalBytes && personal.Length != 0)
            throw new ArgumentException($"Personalization must be {nameof(PersonalBytes)} bytes or empty");

        fixed (byte* i = input)
        fixed (byte* o = output)
        fixed (byte* k = key)
        fixed (byte* s = salt)
        fixed (byte* p = personal)
        {
            var ret = crypto_generichash_blake2b_salt_personal(
                o, (nuint)output.Length,
                i, (ulong)input.Length,
                k, (nuint)key.Length,
                s,
                p);

            return ret == 0;
        }
    }

    public static unsafe bool Init(ref State state, ReadOnlySpan<byte> key, int outputLength)
    {
        if (key.Length > KeyBytesMax)
            throw new ArgumentException("Key too large");

        if (outputLength is < BytesMin or > BytesMax)
            throw new ArgumentException("Output is invalid size");

        fixed (crypto_generichash_blake2b_state* s = &state.Data)
        fixed (byte* k = key)
        {
            var ret = crypto_generichash_blake2b_init(s, k, (nuint)key.Length, (nuint)outputLength);
            return ret == 0;
        }
    }

    public static unsafe bool InitSaltPersonal(
        ref State state,
        ReadOnlySpan<byte> key,
        int outputLength,
        ReadOnlySpan<byte> salt,
        ReadOnlySpan<byte> personal)
    {
        if (key.Length > KeyBytesMax)
            throw new ArgumentException("Key too large");

        if (outputLength is < BytesMin or > BytesMax)
            throw new ArgumentException("Output is invalid size");

        fixed (crypto_generichash_blake2b_state* s = &state.Data)
        fixed (byte* k = key)
        fixed (byte* saltPtr = salt)
        fixed (byte* p = personal)
        {
            var ret = crypto_generichash_blake2b_init_salt_personal(
                s,
                k, (nuint)key.Length,
                (nuint)outputLength,
                saltPtr,
                p);

            return ret == 0;
        }
    }

    public static unsafe bool Update(ref State state, ReadOnlySpan<byte> input)
    {
        fixed (crypto_generichash_blake2b_state* s = &state.Data)
        fixed (byte* i = input)
        {
            var ret = crypto_generichash_blake2b_update(s, i, (nuint)input.Length);
            return ret == 0;
        }
    }

    public static unsafe bool Final(ref State state, Span<byte> output)
    {
        if (output.Length is < BytesMin or > BytesMax)
            throw new ArgumentException("Output is invalid size");

        fixed (crypto_generichash_blake2b_state* s = &state.Data)
        fixed (byte* o = output)
        {
            var ret = crypto_generichash_blake2b_final(s, o, (nuint)output.Length);
            return ret == 0;
        }
    }

    public struct State
    {
        public crypto_generichash_blake2b_state Data;
    }
}
