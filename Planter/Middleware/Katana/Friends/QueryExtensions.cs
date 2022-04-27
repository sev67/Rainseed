using Newtonsoft.Json.Linq;
using Planter.Factories.JObject;
using Newtonsoft.Json;

namespace Planter.Middleware.Katana.Friends;

public static class QueryExtensions
{
        public static int Create(this Kata::Query query, JObject structure)
    {
        Kata::CreateQueryClause clause = new CreateQueryClause(structure);
        return KataExec::QueryExtensions.CreateTable(query, clause.Name, clause.Columns);
    }
    
    public static int Insert(this Kata::Query query, JObject o)
    {
        o = new TableFactory(o);
        Dictionary<string, object?> predicates = new ();
        foreach (JProperty prop in o.Properties())
            predicates.Add(TableFactory.ColumnName(prop.Name), TranslateValue(prop.Value));
        
        return KataExec::QueryExtensions.Insert(query, predicates);
    }

    private static object? TranslateValue(JToken value)
    {
        switch (value.Type)
        {
            case JTokenType.None:
                return string.Empty;
            case JTokenType.Object:
                return value.ToString(Formatting.None);
            case JTokenType.Array:
                return value.ToString(Formatting.None);
            case JTokenType.Constructor:
                return value.ToString(Formatting.None);
            case JTokenType.Property:
                return value.ToString(Formatting.None);
            case JTokenType.Comment:
                return value.ToString(Formatting.None);
            case JTokenType.Integer:
                return value.ToObject<int>();
            case JTokenType.Float:
                return value.ToObject<double>();
            case JTokenType.String:
                return value.ToObject<string>();
            case JTokenType.Boolean:
                return value.ToObject<bool>();
            case JTokenType.Null:
                return null;
            case JTokenType.Undefined:
                return string.Empty;
            case JTokenType.Date:
                return value.ToObject<DateTime>();
            case JTokenType.Raw:
                return value.ToObject<string>();
            case JTokenType.Bytes:
                return value.ToObject<string>();
            case JTokenType.Guid:
                return value.ToObject<string>();
            case JTokenType.Uri:
                return value.ToObject<string>();
            case JTokenType.TimeSpan:
                return value.ToObject<DateTime>();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}