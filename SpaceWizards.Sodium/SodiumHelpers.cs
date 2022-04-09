using System.Buffers;
using System.Text;

namespace SpaceWizards.Sodium;

using static Interop.Libsodium;

public static class SodiumHelpers
{
    static SodiumHelpers()
    {
        SodiumCore.EnsureInit();
    }

    // TODO: hex2bin and base642bin

    /// <summary>
    /// <c>sodium_memcmp</c>
    /// </summary>
    public static unsafe bool MemoryCompare(ReadOnlySpan<byte> b1, ReadOnlySpan<byte> b2)
    {
        if (b1.Length != b2.Length)
            throw new ArgumentException("Both input spans must be the same length.");

        fixed (byte* b1Ptr = b1)
        fixed (byte* b2Ptr = b2)
        {
            return sodium_memcmp(b1Ptr, b2Ptr, (nuint)b1.Length) == 0;
        }
    }

    /// <summary>
    /// <c>sodium_bin2hex</c>
    /// </summary>
    /// <returns>The subsection of <paramref name="hex"/> that was filled, EXCLUDING null terminator.</returns>
    /// <remarks>
    /// If you need a version that gives back a string, just use the BCL <see cref="Convert.ToHexString(ReadOnlySpan{byte})"/> instead.
    /// </remarks>
    public static unsafe Span<byte> Bin2Hex(Span<byte> hex, ReadOnlySpan<byte> bin)
    {
        var needSize = (long)bin.Length * 2 + 1;
        if (hex.Length < needSize)
            throw new ArgumentException("Hex must be at least (bin.Length * 2) + 1 bytes.");

        fixed (byte* hexPtr = hex)
        fixed (byte* binPtr = bin)
        {
            sodium_bin2hex((sbyte*)hexPtr, (nuint)hex.Length, binPtr, (nuint)bin.Length);
        }

        return hex[..(int)(needSize - 1)];
    }

    /// <summary>
    /// <c>sodium_bin2hex</c>, returns string.
    /// </summary>
    public static string Bin2Hex(ReadOnlySpan<byte> bin)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(bin.Length * 2 + 1);

        try
        {
            var data = Bin2Hex(buffer, bin);

            return Encoding.ASCII.GetString(data);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static int Base64EncodedLength(int length, SodiumBase64Variant variant) =>
        checked((int)sodium_base64_ENCODED_LEN((ulong)length, (ulong)variant));

    /// <summary>
    /// <c>sodium_bin2base64</c>
    /// </summary>
    /// <returns>The subsection of <paramref name="b64"/> that was filled, EXCLUDING null terminator.</returns>
    public static unsafe Span<byte> Bin2Base64(Span<byte> b64, ReadOnlySpan<byte> bin, SodiumBase64Variant variant)
    {
        var needSize = sodium_base64_ENCODED_LEN((ulong)bin.Length, (ulong)variant);
        if ((ulong)b64.Length < needSize)
            throw new ArgumentException("B64 is too short for encoded data");

        fixed (byte* b64Ptr = b64)
        fixed (byte* binPtr = bin)
        {
            sodium_bin2base64((sbyte*)b64Ptr, (nuint)b64.Length, binPtr, (nuint)bin.Length, (int)variant);
        }

        return b64[..(int)(needSize - 1)];
    }

    /// <summary>
    /// <c>sodium_bin2hex</c>, returns string.
    /// </summary>
    public static string Bin2Base64(ReadOnlySpan<byte> bin, SodiumBase64Variant variant)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(Base64EncodedLength(bin.Length, variant));

        try
        {
            var data = Bin2Base64(buffer, bin, variant);

            return Encoding.ASCII.GetString(data);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// <c>sodium_increment</c>
    /// </summary>
    public static unsafe void Increment(Span<byte> n)
    {
        fixed (byte* nPtr = n)
        {
            sodium_increment(nPtr, (nuint)n.Length);
        }
    }

    /// <summary>
    /// <c>sodium_add</c>
    /// </summary>
    public static unsafe void Add(Span<byte> a, ReadOnlySpan<byte> b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Both input numbers must be the same length.");

        fixed (byte* aPtr = a)
        fixed (byte* bPtr = b)
        {
            sodium_add(aPtr, bPtr, (nuint)a.Length);
        }
    }

    /// <summary>
    /// <c>sodium_sub</c>
    /// </summary>
    public static unsafe void Sub(Span<byte> a, ReadOnlySpan<byte> b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Both input numbers must be the same length.");

        fixed (byte* aPtr = a)
        fixed (byte* bPtr = b)
        {
            sodium_sub(aPtr, bPtr, (nuint)a.Length);
        }
    }

    /// <summary>
    /// <c>sodium_compare</c>
    /// </summary>
    public static unsafe int Compare(ReadOnlySpan<byte> b1, ReadOnlySpan<byte> b2)
    {
        if (b1.Length != b2.Length)
            throw new ArgumentException("Both input numbers must be the same length.");

        fixed (byte* b1Ptr = b1)
        fixed (byte* b2Ptr = b2)
        {
            return sodium_compare(b1Ptr, b2Ptr, (nuint)b1.Length);
        }
    }

    /// <summary>
    /// <c>sodium_is_zero</c>
    /// </summary>
    public static unsafe bool IsZero(ReadOnlySpan<byte> n)
    {
        fixed (byte* nPtr = n)
        {
            return sodium_is_zero(nPtr, (nuint)n.Length) == 1;
        }
    }

    /// <summary>
    /// <c>sodium_stackzero</c>
    /// </summary>
    public static void StackZero(int len)
    {
        sodium_stackzero((nuint)len);
    }
}

public enum SodiumBase64Variant
{
    Original = sodium_base64_VARIANT_ORIGINAL,
    OriginalNoPadding = sodium_base64_VARIANT_ORIGINAL_NO_PADDING,
    OriginalUrlSafe = sodium_base64_VARIANT_URLSAFE,
    OriginalUrlSafeNoPadding = sodium_base64_VARIANT_URLSAFE_NO_PADDING,
}
