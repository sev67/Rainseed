using Newtonsoft.Json.Linq;
using RainCompiler.Models.Data;
using StandardLib.Friends;
using StandardLib.Types;

namespace RainCompiler.Linker.Streams;

public class RainStreamWriter : RainStream
{
    private readonly BinaryWriter _writer;
    private readonly BinaryReader _reader;
    
    public RainStreamWriter(ReadOnlySpan<char> filePath) 
    : base(filePath)
    {
        _writer = new BinaryWriter(Stream);
        _reader = new BinaryReader(Stream);
    }
    
    public RainStreamWriter(RainStream stream) 
    : base(stream)
    {
        _writer = new BinaryWriter(Stream);
    }

    public void WriteHeader(ReadOnlySpan<char> id, JObjectCollection collection)
    {
        WipeCurrentValue(id);
        _writer.Seek(0, SeekOrigin.End);
        _writer.Write(SIG_ID_MUT);
        _writer.Write(id.ToString());
        _writer.Write(SIG_HEADER_CTXT);
        _writer.Write(collection.Serialize());
    }

    public void CreateBody(ReadOnlySpan<char> id, params JObject[] entries)
    {
        foreach (JObject entry in entries) WipeCurrentValue(id, entry);
            
        _writer.Seek(0, SeekOrigin.End);
        _writer.Write(SIG_ID_MUT);
        _writer.Write(id.ToString());
        foreach (JObject entry in entries)
        {
            _writer.Write(SIG_BODY_CTXT);
            _writer.Write(entry.Serialize());
        }
    }

    override protected void ReleaseUnmanagedResources()
    {
        _writer.Close();
        base.ReleaseUnmanagedResources();
    }
    
    override protected void Disposing()
    {
        _writer.Dispose();
        base.Disposing();
    }
    
    private void WipeCurrentValue(ReadOnlySpan<char> id)
    {
        string? idState = null;
        while (_reader.BaseStream.Position != Stream.Length)
        {
            Int64 sigPtr = _reader.BaseStream.Position;
            switch (_reader.ReadByte())
            {
                case SIG_ID_MUT:
                    idState = _reader.ReadString();
                    break;
                case SIG_HEADER_CTXT:
                    
                    _reader.ReadString();
                    
                    if (id == idState)
                    {
                        _writer.BaseStream.Position = sigPtr;
                        _writer.Write(SIG_SKIP);
                        return;
                    }
                    break;
                case SIG_BODY_CTXT:
                case SIG_SKIP:
                    _reader.ReadString();
                    break;
            }
        }
    }

    private void WipeCurrentValue(ReadOnlySpan<char> id, JObject entry)
    {
        string? idState = null;
        while (_reader.BaseStream.Position != Stream.Length)
        {
            Int64 sigPtr = _reader.BaseStream.Position;
            switch (_reader.ReadByte())
            {
                case SIG_ID_MUT:
                    idState = _reader.ReadString();
                    break;
                case SIG_HEADER_CTXT:
                case SIG_SKIP:
                    _reader.ReadString();
                    break;
                case SIG_BODY_CTXT:
                    string bodyBuffer = _reader.ReadString();
                    
                    if (id == idState 
                        && bodyBuffer.Deserialize()?.GetPropertyValue<string>("uid") 
                        == entry.GetPropertyValue<string>("uid"))
                    {
                        _writer.BaseStream.Position = sigPtr;
                        _writer.Write(SIG_SKIP);
                        return;
                    }
                    break;
            }
        }
    }
}