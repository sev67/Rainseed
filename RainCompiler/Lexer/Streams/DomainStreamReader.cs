namespace RainCompiler.Lexer.Streams;

abstract public class DomainStreamReader<T> : DomainStreamReader, IDomainStreamReader<T>
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="stream">Stream to read.</param>
    protected DomainStreamReader(in Stream stream)
    : base (stream)
    {/* ... */}

    new virtual public IEnumerable<T> Read()
    {
        foreach (string buffer in base.Read())
        {
            T? value = Translate(buffer);
            if (value is null) continue;
            
            yield return value;
        }
    }

    abstract protected T? Translate(string buffer);
}

public class DomainStreamReader : IDomainStreamReader, IDisposable
{
    private readonly StreamReader _reader;
    private readonly Stream _stream;
    private bool _disposed;
    private uint _line;

    public uint Line
    => _line;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="stream">Stream to read.</param>
    protected DomainStreamReader(in Stream stream)
    {
        _stream = stream;
        _reader = new StreamReader(stream);
        _line = 0;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    protected DomainStreamReader(DomainStreamReader self)
    : this(in self._stream)
    {
        Flush();
    }

    virtual protected IEnumerable<string> Read()
    {
        string? buffer;
        while ((buffer = _reader.ReadLine()) != null)
        {
            ++_line;
            yield return buffer;
        }

        Flush();
    }

    public void Flush()
    {
        _stream.Position = 0;
        _reader.DiscardBufferedData();
        _line = 0;
    }

    public void Jump(uint line)
    {
        Flush();
        
        if (line == 0) return;
        foreach (string _ in Read())
        {
            if (_line == line) break;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        _reader.Close();
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        ReleaseUnmanagedResources();
        if (disposing)
        {
            _reader.Dispose();
        }

        _disposed = true;
    }

    ~DomainStreamReader()
    {
        Dispose(false);
    }
}

public interface IDomainStreamReader<out T> : IDomainStreamReader
{
    IEnumerable<T> Read();
}

public interface IDomainStreamReader
{
    uint Line { get; }
    void Flush();
    void Jump(uint line);
}