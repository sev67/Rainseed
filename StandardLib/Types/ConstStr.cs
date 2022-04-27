namespace StandardLib.Types;

sealed public class ConstStr
{
    private readonly string _buffer;

    private ConstStr(ReadOnlySpan<char> value)
        => _buffer = value.ToString();

    override public string ToString() 
        => _buffer;

    public static implicit operator ConstStr(string value)
        => new (value);
    
    public static implicit operator ConstStr(ReadOnlySpan<char> value)
        => new (value);
    
    public static implicit operator ConstStr(char[] value)
        => new (value);
    
    public static implicit operator string(ConstStr self)
        => self._buffer;
    
    public static implicit operator ReadOnlySpan<char>(ConstStr self)
        => self._buffer;
    
    public static implicit operator char[](ConstStr self)
        => self._buffer.ToCharArray();
}