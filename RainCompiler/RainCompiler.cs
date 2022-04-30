using Newtonsoft.Json.Linq;
using RainCompiler.Builder;
using RainCompiler.Facade;
using RainCompiler.Lexer;
using RainCompiler.Lexer.Streams;
using RainCompiler.Linker;
using RainCompiler.Linker.Streams;
using StandardLib.Middleware.Disk;

namespace RainCompiler;

public class RainCompiler
{
    private readonly Compiler _compiler;

    public RainCompiler(IDomainStreamReader<JObject> dumpStream, IAppData appData)
    {
        _compiler = new Compiler(dumpStream, appData);
    }
    
    public void Preprocess()
    => _compiler.Lex();
    
    public RainStream Build()
    => _compiler.Link();
    
    public RainStream Rebuild()
    => _compiler.Lex().Translate().Link();

    public bool HasBuilt()
    => _compiler.Lexer.ListContexts().Any() && _compiler.Translator.ListObjectFiles().Any();

}