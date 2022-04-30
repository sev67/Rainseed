using Newtonsoft.Json.Linq;
using RainCompiler.Builder;
using RainCompiler.Linker.Streams;
using RainCompiler.Models.Data;
using StandardLib.Types;

namespace RainCompiler.Linker;

public class RainLinker : Linker<JObject>
{
    private readonly RainStreamWriter _writer;
    
    override sealed protected ReadOnlySpan<char> ArtifactsPath 
    => "build/out/rain.bin";

    public RainLinker(IBuilder<JObject> builder)
    : base(builder)
    {
        _writer = new RainStreamWriter(ArtifactsPath);
    }
    
    override protected void LinkObject(ReadOnlySpan<char> objectId, IEnumerable<JObject> obj, ref JObjectCollection header)
    {
        foreach (JObject symbol in obj)
        {
            foreach (JProperty property in symbol.Properties())
            {
                if (header.Contains(property)) continue;
                header.Columns += (JObjectColumn) property;
            }
            
            _writer.CreateBody(objectId, symbol);
        }
    }

    override protected void LinkHeader(ReadOnlySpan<char> objectId, in JObjectCollection header)
    => _writer.WriteHeader(objectId, header);
}