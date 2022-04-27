namespace StandardLib.Singletons;

/// <summary>
/// Provides outputs for the program.
/// </summary>
/// <remarks>Outputs should only be added in program.cs.</remarks>
public static class OutputMarshal
{
    public delegate void Out(string buffer);

    private static Out _slots;
    private readonly static Out NoOp = _ => { };

    static OutputMarshal()
    {
        _slots = NoOp;
    }

    public static void Slot(Out slot)
        => _slots += slot;

    public static void Signal(string buffer)
        => _slots(buffer);
}