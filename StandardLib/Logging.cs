namespace StandardLib;

sealed public class Logging
{
    private readonly Stream _stream;
    private readonly StreamWriter _log;

    public Logging(string logPath)
    {
        _stream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Read);
        _log = new StreamWriter(_stream);
    }

    public void Write(string buffer)
    => _log.WriteLine(DateTime.Now.ToString("g") + " : " + buffer.Trim(new [] { '\r', '\n', ' '}));

    public void Flush()
    {
        _log.Flush();
    }

    ~Logging()
    {
        _log.Flush();
        
        _log.Close();
        _stream.Close();
        
        _log.Dispose();
        _stream.Dispose();
    }
}