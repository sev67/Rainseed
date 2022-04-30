using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RainCompiler.Lexer.Streams;

sealed public class DumpStreamReader : DomainStreamReader<JObject>
{
    public DumpStreamReader(in Stream stream) 
    : base(in stream)
    {/* ... */}

    override protected JObject? Translate(string buffer)
    {
        try
        {
            return JsonConvert.DeserializeObject<JObject>(buffer.Trim(new[] {' ', ','}));
        }
        catch
        {
            return null;
        }
    }
}