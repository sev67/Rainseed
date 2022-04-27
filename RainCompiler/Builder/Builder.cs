using RainCompiler.Lexer;
using StandardLib.Middleware.Disk;

namespace RainCompiler.Builder;

abstract public class Builder<T> : IBuilder<T>
{
    private readonly ILexer<T> _lexer;
    private readonly IAppData _appData;
    
    abstract protected string ArtifactsPath { get; }

    protected Builder(ILexer<T> lexer, IAppData appData)
    {
        _lexer = lexer;
        _appData = appData;
    }

    public IEnumerable<T> ReadObjectFile(string file) => throw new NotImplementedException();

    public IEnumerable<string> ListObjectFiles() => throw new NotImplementedException();

    public void Compile()
    {
        foreach (string context in _lexer.ListContexts())
        {
            T obj = CompileContext(_lexer.ReadContext(context));
            WriteObjectArtifact(GetObjectSector(in obj), obj);
        }
    }
    
    protected void WriteObjectArtifact(string sector, string artifactBuffer)
    {
        _appData.Write($"{ArtifactsPath}/{sector}", artifactBuffer, true);
    }
    
    virtual protected void WriteObjectArtifact(string sector, T artifact)
    {
        if (artifact is null) return;
        WriteObjectArtifact(sector, artifact.ToString());
    }

    abstract protected T CompileContext(IEnumerable<T> context);

    abstract protected string GetObjectSector(in T obj);
    
    
}

public interface IBuilder<out T> : IBuilder
{
    IEnumerable<T> ReadObjectFile(string file);
}

public interface IBuilder
{
    IEnumerable<string> ListObjectFiles();
    void Compile();
}