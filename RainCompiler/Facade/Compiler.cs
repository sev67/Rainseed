using Newtonsoft.Json.Linq;
using RainCompiler.Builder;
using RainCompiler.Lexer;
using RainCompiler.Lexer.Streams;
using RainCompiler.Linker;
using RainCompiler.Linker.Streams;
using StandardLib.Middleware.Disk;

namespace RainCompiler.Facade;

sealed public class Compiler
{
    private readonly ILexer<JObject> _lexer;
    private readonly IBuilder<JObject> _builder;
    private readonly ILinker _linker;

    public ILexer<JObject> Lexer 
        => _lexer;
    
    public IBuilder<JObject> Translator 
        => _builder;
    
    public Compiler(IDomainStreamReader<JObject> dumpStream, IAppData appData)
    {
        _lexer = new DumpLexer(appData, dumpStream);
        _builder = new RainBuilder(_lexer, appData);
        _linker = new RainLinker(_builder);
    }

    public Compiler Lex()
    {
        _lexer.Lex();
        return this;
    }

    public Compiler Translate()
    {
        _builder.Compile();
        return this;
    }

    public RainStream Link()
    {
        return _linker.Link();
    }
}