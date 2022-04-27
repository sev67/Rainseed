using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RainCompiler.Lexer.Streams;
using StandardLib.Friends;
using StandardLib.Middleware.Disk;

namespace RainCompiler.Lexer;

public class DumpLexer : Lexer<JObject>
{
    override protected string ArtifactsPath 
    => "build/tree";
    
    override protected string LockId 
    => nameof(DumpLexer).ToLower();
    
    public DumpLexer(IAppData appData, IDomainStreamReader<JObject> reader) 
    : base(appData, reader)
    {/* ... */}

    override protected void BuildContextReader(in FileStream fStream, out IDomainStreamReader<JObject> reader)
    => reader = new DumpStreamReader(fStream);
    
    override protected void LexValue(JObject value, IAppData appData)
    {
        string? uid = value.GetPropertyValue<string>("uid");
        appData.Write($"{ArtifactsPath}/{uid}", value.ToString(Formatting.None), true);
    }
}