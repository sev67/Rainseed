using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StandardLib.Friends;

public static class Strings
{
    public static JsonSerializerSettings Settings => new ()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Include
    };
    
    public static string AsPath(this string value)
    {
        string BuildPath(char separator, string path) 
            =>Path.Combine(path.Split(separator, StringSplitOptions.RemoveEmptyEntries));

        return BuildPath(value.Contains('/') ? '/' : '\\', value);
    }

    public static string Serialize(this object obj)
        => JsonConvert.SerializeObject(obj);
    
    public static T? Deserialize<T>(this string value)
        => JsonConvert.DeserializeObject<T>(value);
    
    public static JObject? Deserialize(this string value)
        => JsonConvert.DeserializeObject<JObject>(value);
}