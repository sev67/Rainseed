using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RainCompiler.Lexer;
using RainCompiler.Lexer.Streams;
using StandardLib.Friends;
using StandardLib.Middleware.Disk;
using StandardLib.Singletons;
using StandardLib.Types;

namespace RainCompiler.Builder;

public class RainBuilder : Builder<JObject>
{
    private readonly IAppData _appData;
    
    override protected string ArtifactsPath 
        => "build/obj";

    public RainBuilder(ILexer<JObject> lexer, IAppData appData)
    : base(lexer, appData)
    {
        _appData = appData;
    }
    
    override public IEnumerable<JObject> ReadObjectFile(string file)
    {
        string path = _appData.GetLogicalPath($"{ArtifactsPath}/{file}");
        using FileStream fStream = new(path, FileMode.Open, FileAccess.Read);
        using DomainStreamReader<JObject> reader = new DumpStreamReader(fStream);
        
        foreach (JObject value in reader.Read()) yield return value;
    }
    
    override public IEnumerable<ConstStr> ListObjectFiles() 
        => _appData.ListFiles(ArtifactsPath);

    override protected JObject CompileContext(IEnumerable<JObject> context)
    {
        JObject objectBuilder = new ();
        foreach (JObject obj in context) Consolidate(ref objectBuilder, obj.Properties());
        return objectBuilder;
    }
    
    override protected string GetObjectSector(in JObject obj)
    {
        static string HashByDefinition(in JObject obj)
        {
            string buffer = string.Empty;
            foreach (JProperty property in obj.Properties()) buffer += property.Name;
            return Sha256.Hash(buffer);
        }
        
        return obj.GetPropertyValue<string>("dgraph.type") ?? HashByDefinition(obj);
    }

    override protected void WriteObjectArtifact(string sector, JObject artifact)
    => base.WriteObjectArtifact(sector, artifact.ToString(Formatting.None));
    


    /// <summary>
    /// Merge properties into builder object.
    /// </summary>
    private static void Consolidate(ref JObject builder, IEnumerable<JProperty> properties)
    {
        foreach (JProperty property in properties)
        {
            if (builder.ContainsKey(property.Name))
            {
                OverloadProperty(ref builder, property);
            }
            else
            {
                builder.Add(property.Name, property.Value);
            }
        }
    }

    private static void OverloadProperty(ref JObject builder, JProperty property)
    {
        static JToken? Multicast(JToken? parent, JToken child)
        {
            Vector<JObject>? signal = parent?.ToObject<Vector<JObject>>();
            JObject? slot = child.ToObject<Vector<JObject>>()?.First();
            if (signal is null | slot is null) return parent;

            bool match = false;
            foreach (JObject o in signal!)
            {
                if (o["uid"]!.Value<string>() != slot!["uid"]!.Value<string>()) continue;

                match = true;
                break;
            }

            if (!match) signal += slot!;
            return JToken.FromObject(signal);
        }
        
        if (builder[property.Name]?.Type == JTokenType.Array && property.Value.Type == JTokenType.Array)
        {
            builder[property.Name] = Multicast(builder[property.Name], property.Value);
        }
        else
        {
            builder[property.Name] = property.Value;
        }
    }
}