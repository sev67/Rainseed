using System.Collections.Immutable;
using System.Numerics;
using Newtonsoft.Json.Linq;
using RainCompiler.Linker.Streams;
using StandardLib.Friends;

namespace RainBucket.Crawl;

public class Spider : ISpider
{
    private readonly RainStreamReader _reader;
    
    public Spider(RainStream stream)
    {
        _reader = new RainStreamReader(stream);
    }
    
    public JObject Crawl()
    {
        ImmutableArray<string> ids = _reader.GetIds().ToImmutableArray();
        foreach (string id in ids)
        {
            if (!_reader.GetHeader(id)?.HasReferenceTypes() ?? true) continue;

            foreach (JObject entity in _reader.GetBody(id))
            {
                foreach (JProperty property in entity.Properties())
                {
                    if (property.Type != JTokenType.Array) continue;

                    JObject[] refrences = property.Value.ToObject<JObject[]>();
                    foreach (JObject o in refrences)
                    {
                        string refrenceUid = o.GetPropertyValue<string>("uid");

                        Int64 ptr = _reader.Bookmark();

                        JObject refedObj = _reader.GetEntity(refrenceUid);

                        _reader.Bookmark(ptr);
                    }
                }
            }
        }
    }

    public void Flush()
    {
        throw new NotImplementedException();
    }
}

public interface ISpider
{
    JObject Crawl();
    void Flush();
}