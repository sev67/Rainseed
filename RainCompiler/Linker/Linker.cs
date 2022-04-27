using RainCompiler.Builder;
using RainCompiler.Linker.Streams;
using RainCompiler.Models.Data;

namespace RainCompiler.Linker;

abstract public class Linker<T> : ILinker
{
    private readonly IBuilder<T> _builder;

    public Linker(IBuilder<T> builder)
    {
        _builder = builder;
    }
    
    abstract public RainStreamReader Link();
}

public interface ILinker
{
    RainStreamReader Link();
}