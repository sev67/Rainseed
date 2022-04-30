using Newtonsoft.Json.Linq;
using RainCompiler.Models.Data;
using StandardLib.Friends;

namespace RainCompiler.Linker.Streams;

public class RainStreamWriter : RainStream
{
    private readonly BinaryWriter _writer;
    
    public RainStreamWriter(ReadOnlySpan<char> filePath) 
    : base(filePath)
    {
        _writer = new BinaryWriter(Stream);
    }
    
    public RainStreamWriter(RainStream stream) 
    : base(stream)
    {
        _writer = new BinaryWriter(Stream);
    }

    public void WriteHeader(ReadOnlySpan<char> id, params JObjectCollection[] collections)
    {
        _writer.Seek(0, SeekOrigin.End);
        _writer.Write(SIG_ID_MUT);
        _writer.Write(id.ToString());
        foreach (JObjectCollection collection in collections)
        {
            _writer.Write(SIG_HEADER_CTXT);
            _writer.Write(collection.Serialize());
        }
    }

    public void CreateBody(ReadOnlySpan<char> id, params JObject[] entries)
    {
        _writer.Seek(0, SeekOrigin.End);
        _writer.Write(SIG_ID_MUT);
        _writer.Write(id.ToString());
        foreach (JObject entry in entries)
        {
            _writer.Write(SIG_BODY_CTXT);
            _writer.Write(entry.Serialize());
        }
    }
}