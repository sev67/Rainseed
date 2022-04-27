using Newtonsoft.Json.Linq;

namespace Planter.Factories.JObject;

public class TableFactory : Newtonsoft.Json.Linq.JObject
{
    private readonly Newtonsoft.Json.Linq.JObject _jObject;
    
    public TableFactory(Newtonsoft.Json.Linq.JObject obj)
    {
        _jObject = obj;

        foreach (JProperty property in _jObject.Properties())
        {
            switch (property.Name)
            {
                case "uid":
                    Translate("uid", "dgraph_id");
                    break;
                case "namespace":
                case "dgraph.type":
                    break;
                default:
                    Translate(property.Name, ColumnName(property.Name));
                    break;
            }
        }
    }

    private void Translate(string name, string? newName = null)
    {
        newName ??= name;
        if (_jObject.ContainsKey(name)) Add(newName, _jObject.Property(name)?.Value);
    }
    
    public static string TableName(string name)
    {
        name = name.ToLower().Trim();
        return name.Replace(".", "_");
    }

    public static string ColumnName(string name)
    {
        string buffer = string.Empty;
        
        foreach (char c in name)
        {
            if (char.IsUpper(c))
            {
                buffer += $"_{c}".ToLower();
                continue;
            }

            buffer += c;
        }

        return buffer;
    }
}