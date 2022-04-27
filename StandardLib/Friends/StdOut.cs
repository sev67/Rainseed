namespace StandardLib.Friends;

/// <summary>
/// Standard output facade for handling outputs intuitively.
/// </summary>
public static class StdOut
{
    /// <summary>
    /// Binds a procedure to the standard output signal.
    /// </summary>
    /// <remarks>This action cannot be undone.</remarks>
    public static void BindCout(Singletons::OutputMarshal.Out slot)
        => Singletons::OutputMarshal.Slot(slot);

    /// <summary>
    /// Write buffer of text to standard output.
    /// </summary>
    public static void Cout(this string buffer)
        => Singletons::OutputMarshal.Signal(buffer);
    
    /// <summary>
    /// End line character sequence depending on the environment.
    /// </summary>
    public static string Endl(this string buffer)
        => $"{buffer}{Environment.NewLine}";
}