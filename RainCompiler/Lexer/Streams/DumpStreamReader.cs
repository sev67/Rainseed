using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RainCompiler.Lexer.Streams;

sealed public class DumpStreamReader : DomainStreamReader<JObject>
{
    public DumpStreamReader(in Stream stream) 
    : base(in stream)
    {/* ... */}

    override protected JObject? Translate(string buffer)
    => JsonConvert.DeserializeObject<JObject>(buffer.Trim(new[] {' ', ','}));
}