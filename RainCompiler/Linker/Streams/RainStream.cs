using StandardLib.Types;

namespace RainCompiler.Linker.Streams;

public class RainStream : IDisposable
{
    public const byte SIG_HEADER_CTXT = 0x00;
    public const byte SIG_BODY_CTXT = 0x01;
    public const byte SIG_ID_MUT = 0x02;
    public const byte SIG_SKIP = 0x03;
    
    private readonly Stream _stream;
    private bool _disposed;

    public long Position 
    => _stream.Position;
    
    public long Length 
    => _stream.Length;
    
    protected Stream Stream 
    => _stream;

    public RainStream(Stream stream)
    {
        _stream = stream;
    }
    
    public RainStream(ReadOnlySpan<char> filePath)
    : this(new FileStream(filePath.ToString(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
    {/* ... */}

    virtual public void Flush()
    {
        _stream.Flush();
        _stream.Position = 0;
    }

    virtual protected void ReleaseUnmanagedResources()
    {
        _stream.Close();
    }
    
    virtual protected void Disposing()
    {
        _stream.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        ReleaseUnmanagedResources();
        if (disposing) Disposing();
        _disposed = true;
    }

    ~RainStream()
    {
        Dispose(false);
    }

    public static implicit operator Stream(RainStream self)
    => self._stream;
}