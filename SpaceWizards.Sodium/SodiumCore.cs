using static SpaceWizards.Sodium.Interop.Libsodium;

namespace SpaceWizards.Sodium;

public static class SodiumCore
{
    /// <summary>
    /// Directly call <see cref="sodium_init"/>.
    /// </summary>
    /// <returns>0 on success, 1 if already initialized, -1 on initialize failure.</returns>
    /// <seealso cref="EnsureInit"/>
    public static int Init()
    {
        return sodium_init();
    }

    /// <summary>
    /// Try to ensure libsodium is initialized, throwing if it fails to initialize.
    /// </summary>
    /// <exception cref="SodiumInitException">Thrown if initialization of libsodium failed.</exception>
    public static void EnsureInit()
    {
        if (Init() == -1)
            throw new SodiumInitException("Failed to init libsodium!");
    }
}
