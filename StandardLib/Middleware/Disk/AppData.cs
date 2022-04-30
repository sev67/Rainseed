using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using StandardLib.Friends;
using StandardLib.Types;

namespace StandardLib.Middleware.Disk;

sealed public class AppData : IAppData
{
    private readonly string _namespace;

    public AppData(string @namespace)
    {
        _namespace = @namespace;
    }

    public string Root
    => _namespace;
    
    public string GetLogicalPath(string filePath)
    => Path.Combine(_namespace, filePath.AsPath());

    public string Read(string filePath)
    {
        filePath = GetLogicalPath(filePath);
        using FileStream fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using StreamReader streamReader = new StreamReader(fstream);
        return streamReader.ReadToEnd();
    }

    public T Read<T>(string filePath)
    where T : class
    {
        filePath = GetLogicalPath(filePath);
        using FileStream fstream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        byte[] buffer = new byte[fstream.Length];
        fstream.Read(buffer, 0, buffer.Length);

        MemoryStream ms = new MemoryStream(buffer);
        using (BsonReader reader = new BsonReader(ms))
        {
            JsonSerializer serializer = new ();
            T? obj = serializer.Deserialize<T>(reader);

            if (obj is null) throw new Exception();
            return obj;
        }
    }

    public void Write(string filePath, string buffer, bool append = false)
    {
        filePath = GetLogicalPath(filePath);
        {
            string? basePath = Path.GetDirectoryName(filePath);
            if (basePath is not null && !Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
        }
        
        using FileStream fstream = new (filePath, (append) ? FileMode.Append : FileMode.Create, FileAccess.Write, 
            FileShare.Read);
        using StreamWriter streamWriter = new (fstream);
        if (append) buffer += Environment.NewLine;
        streamWriter.Write(buffer);
    }
    
    public void Write<T>(string filePath, T data)
    where T : class
    {
        if (data is string buffer)
        {
            Write(filePath, buffer, false);
            return;
        }
        
        filePath = GetLogicalPath(filePath);
        {
            string? basePath = Path.GetDirectoryName(filePath);
            if (basePath is not null && !Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
        }
        
        MemoryStream ms = new ();
        using (BsonDataWriter writer = new (ms))
        {
            JsonSerializer serializer = new ();
            serializer.Serialize(writer, data);
        }

        byte[] binary = ms.ToArray();
        using FileStream fstream = new (filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        fstream.Write(binary, 0, binary.Length);
    }

    public IEnumerable<ConstStr> ListFiles(string filePath)
    {
        filePath = GetLogicalPath(filePath);
        if (!Directory.Exists(filePath)) yield break;
        
        DirectoryInfo place = new(filePath);
        foreach (FileInfo i in place.GetFiles()) yield return i.Name;
    }
    
    public IEnumerable<ConstStr> ListPaths(string filePath)
    {
        filePath = GetLogicalPath(filePath);
        if (!Directory.Exists(filePath)) yield break;
        
        DirectoryInfo place = new(filePath);
        foreach (DirectoryInfo i in place.GetDirectories()) yield return i.Name;
    }

    public bool Has(string path)
    {
        path = GetLogicalPath(path);
        return File.Exists(path) || Directory.Exists(path);
    }

    public void Wipe(string path)
    {
        path = GetLogicalPath(path);
        if (File.Exists(path)) File.Delete(path);
    }
}

public interface IAppData
{
    string Root { get; }
    string GetLogicalPath(string filePath);
    string? Read(string filePath);
    T Read<T>(string filePath) where T : class;
    void Write(string filepath, string buffer, bool append = false);
    void Write<T>(string filepath, T buffer) where T : class;
    IEnumerable<ConstStr> ListFiles(string filePath);
    IEnumerable<ConstStr> ListPaths(string filePath);
    bool Has(string path);
    void Wipe(string path);
}