using Newtonsoft.Json.Linq;

namespace StandardLib.Friends;

public static class JObjects
{
    public static T? GetPropertyValue<T>(this JObject o, string property)
    where T : class
    {
        JToken? prop = o[property];
        return prop?.Value<T>();
    }
}