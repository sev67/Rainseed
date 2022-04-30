using Newtonsoft.Json.Linq;
using RainCompiler.Builder;
using RainCompiler.Linker.Streams;
using RainCompiler.Models.Data;
using StandardLib.Friends;
using StandardLib.Types;

namespace RainCompiler.Linker;

public class RainLinker : Linker<JObject>
{
    private RainStreamWriter _writer;
    
    override sealed protected ReadOnlySpan<char> ArtifactsPath 
    => "rain.bin";

    public RainLinker(IBuilder<JObject> builder)
    : base(builder)
    {
        if (File.Exists(ArtifactsPath.ToString())) File.Delete(ArtifactsPath.ToString());
    }

    override protected void PreLinking()
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

    override protected void PostLinking()
    {
        _writer.Flush();
        _writer.Dispose();
    }
}