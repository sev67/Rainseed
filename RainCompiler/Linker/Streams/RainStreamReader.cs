using System.Numerics;
using Newtonsoft.Json.Linq;
using RainCompiler.Models.Data;
using StandardLib.Friends;
using StandardLib.Types;

namespace RainCompiler.Linker.Streams;

public class RainStreamReader : RainStream
{
    private readonly BinaryReader _reader;
    
    public RainStreamReader(ReadOnlySpan<char> filePath)
    : base(filePath)
    {
        _reader = new BinaryReader(Stream);
    }
    
    public RainStreamReader(RainStream stream)
    : base(stream)
    {
        _reader = new BinaryReader(Stream);
    }

    JObjectCollection? GetHeader(ConstStr id)
    {
        Flush();
        string? idState = null;
        
        while (_reader.BaseStream.Position != Stream.Length)
        {
            switch (_reader.ReadByte())
            {
                case SIG_ID_MUT:
                    idState = _reader.ReadString();
                    break;
                case SIG_HEADER_CTXT:
                    string headerBuffer = _reader.ReadString();
                    if (id == idState) return headerBuffer.Deserialize<JObjectCollection>();
                    break;
                case SIG_BODY_CTXT:
                    _reader.ReadString();
                    break;
            }
        }

        return null;
    }

    IEnumerable<JObject> GetBody(ConstStr id)
    {
        Flush();
        string? idState = null;

        while (_reader.BaseStream.Position != Stream.Length)
        {
            switch (_reader.ReadByte())
            {
                case SIG_ID_MUT:
                    idState = _reader.ReadString();
                    break;
                case SIG_HEADER_CTXT:
                    _reader.ReadString();
                    break;
                case SIG_BODY_CTXT:
                    string bodyBuffer = _reader.ReadString();
                    if (id == idState)
                    {
                        JObject? jobj = bodyBuffer.Deserialize();
                        if (jobj is null) break;
                        yield return jobj;
                    }
                    break;
            }
        }
    }
    
    override protected void ReleaseUnmanagedResources()
    {
        _reader.Close();
        base.ReleaseUnmanagedResources();
    }
    
    override protected void Disposing()
    {
        _reader.Dispose();
        base.Disposing();
    }
}