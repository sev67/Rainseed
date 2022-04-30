using RainCompiler.Builder;
using RainCompiler.Linker.Streams;
using RainCompiler.Models.Data;
using StandardLib.Types;

namespace RainCompiler.Linker;

abstract public class Linker<T> : ILinker
{
    private readonly IBuilder<T> _builder;
    
    abstract protected ReadOnlySpan<char> ArtifactsPath { get; }

    public Linker(IBuilder<T> builder)
    {
        _builder = builder;
    }

    public RainStream Link()
    {
        PreLinking();
        foreach (ConstStr objectFile in _builder.ListObjectFiles())
        {
            JObjectCollection header = new ();
            LinkObject(objectFile, _builder.ReadObjectFile(objectFile), ref header);
            LinkHeader(objectFile, in header);
        }
        PostLinking();
        
        return new RainStream(ArtifactsPath);
    }

    abstract protected void PreLinking();
    
    abstract protected void LinkObject(ReadOnlySpan<char> objectId, IEnumerable<T> obj, ref JObjectCollection header);
    
    abstract protected void LinkHeader(ReadOnlySpan<char> objectId, in JObjectCollection header);

    abstract protected void PostLinking();
}

public interface ILinker
{
    RainStream Link();
}