namespace SpaceWizards.Sodium;

using static Interop.Libsodium;

public static class CryptoPwHash
{
    static CryptoPwHash()
    {
        SodiumCore.EnsureInit();
    }

    public const int SaltBytes = (int)crypto_pwhash_SALTBYTES;
    public const int PasswdMin = (int)crypto_pwhash_PASSWD_MIN;
    public const int BytesMin = (int)crypto_pwhash_BYTES_MIN;

    public const uint MemLimitMin = crypto_pwhash_MEMLIMIT_MIN;
    public const ulong MemLimitMax = crypto_pwhash_MEMLIMIT_MAX;
    public const long MemLimitInteractive = crypto_pwhash_MEMLIMIT_INTERACTIVE;
    public const long MemLimitModerate = crypto_pwhash_MEMLIMIT_MODERATE;
    public const long MemLimitSensitive = crypto_pwhash_MEMLIMIT_SENSITIVE;

    public const uint OpsLimitMin = crypto_pwhash_OPSLIMIT_MIN;
    public const ulong OpsLimitMax = crypto_pwhash_OPSLIMIT_MAX;
    public const long OpsLimitInteractive = crypto_pwhash_OPSLIMIT_INTERACTIVE;
    public const long OpsLimitModerate = crypto_pwhash_MEMLIMIT_MODERATE;
    public const long OpsLimitSensitive = crypto_pwhash_MEMLIMIT_SENSITIVE;

    public static unsafe bool Derive(
        Span<byte> key,
        ReadOnlySpan<byte> salt,
        ReadOnlySpan<byte> password,
        ulong opsLimit = OpsLimitInteractive,
        ulong memLimit = MemLimitInteractive,
        PwHashAlgorithm alg = PwHashAlgorithm.AlgDefault)
    {
        // Libsodiums pwhash max output length is greater than Int32.MaxValue
        if (key.Length < BytesMin)
            throw new ArgumentException($"Output must be at least {nameof(BytesMin)} bytes");

        if (salt.Length != SaltBytes)
            throw new ArgumentException($"Salt must be {nameof(SaltBytes)} bytes");

        // Libsodiums pwhash max password length is greater than Int32.MaxValue
        if (password.Length < PasswdMin)
            throw new ArgumentException($"Output must be at least {nameof(PasswdMin)} bytes");

        if (opsLimit is < OpsLimitMin or > OpsLimitMax)
            throw new ArgumentException("OpsLimit is invalid size");

        if (memLimit is < MemLimitMin or > MemLimitMax)
            throw new ArgumentException("MemLimit is invalid size");

        fixed (byte* k = key)
        fixed (byte* s = salt)
        fixed (byte* p = password)
        {
            var ret = crypto_pwhash(
                k,
                (ulong)key.Length,
                (sbyte*)p,
                (ulong)password.Length,
                s,
                opsLimit,
                (nuint)memLimit,
                (int)alg);

            return ret == 0;
        }
    }
}

public enum PwHashAlgorithm
{
    AlgDefault = crypto_pwhash_ALG_DEFAULT,
    AlgArgon2I13 = crypto_pwhash_ALG_ARGON2I13,
    AlgArgon2Id13 = crypto_pwhash_ALG_ARGON2ID13,
}
