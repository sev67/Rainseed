using RainCompiler.Lexer.Streams;
using StandardLib.Middleware.Disk;
using StandardLib.Types;

namespace RainCompiler.Lexer;

abstract public class Lexer<T> : ILexer<T>
{
    private readonly IAppData _appData;
    private readonly IDomainStreamReader<T> _reader;
    
    abstract protected string ArtifactsPath { get; }
    abstract protected string LockId { get; }
    
    protected Lexer(IAppData appData, IDomainStreamReader<T> reader)
    {
        _appData = appData;
        _reader = reader;
    }
    
    abstract protected void BuildContextReader(in FileStream fStream, out IDomainStreamReader<T> reader);

    abstract protected void LexValue(T value, IAppData appData);

    public void Lex()
    {
         ConstStr lockFile = $".lock.{LockId}";
         
         if (_appData.Has(lockFile))
         {
             uint line = uint.Parse(_appData.Read(lockFile) ?? "0");
             _reader.Jump(line);
         }
         
         foreach (T value in _reader.Read())
         {
             LexValue(value, _appData);
             _appData.Write(lockFile, _reader.Line.ToString());
         }
         
         _appData.Wipe(lockFile);
    }    
    
    public IEnumerable<T> ReadContext(string context) {
        string path = _appData.GetLogicalPath($"{ArtifactsPath}/{context}");
        using FileStream fStream = new(path, FileMode.Open, FileAccess.Read);

        BuildContextReader(fStream, out IDomainStreamReader<T> reader);
        foreach (T value in reader.Read()) yield return value;
    }

    public IEnumerable<string> ListContexts() 
    => _appData.ListFiles(ArtifactsPath);
}

public interface ILexer<out T> : ILexer
{
    IEnumerable<T> ReadContext(string context);
}

public interface ILexer
{
    void Lex();
    IEnumerable<string> ListContexts();
}