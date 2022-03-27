namespace SpaceWizards.Sodium;

[Serializable]
public sealed class SodiumInitException : Exception
{
    public SodiumInitException()
    {
    }

    public SodiumInitException(string message) : base(message)
    {
    }

    public SodiumInitException(string message, Exception inner) : base(message, inner)
    {
    }
}
