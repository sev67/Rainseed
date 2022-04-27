using Newtonsoft.Json.Linq;
using Planter.Factories.JObject;

namespace Planter.Middleware.Katana;

public class Column : Kata::AbstractCreateClause.Column
{
    public Column(JProperty predicate)
    {
        Name = TableFactory.ColumnName(predicate.Name);
        Type = TranslateType(predicate.Value.Type);
    }

    private static Kata::AbstractCreateClause.Types TranslateType(JTokenType type)
    => type switch
    {
        JTokenType.Array => Kata::AbstractCreateClause.Types.Text,
        JTokenType.Boolean => Kata::AbstractCreateClause.Types.Bool,
        JTokenType.Bytes => Kata::AbstractCreateClause.Types.Text,
        JTokenType.Date => Kata::AbstractCreateClause.Types.DateTime,
        JTokenType.Float => Kata::AbstractCreateClause.Types.Decimal,
        JTokenType.Guid => Kata::AbstractCreateClause.Types.Text,
        JTokenType.Integer => Kata::AbstractCreateClause.Types.Number,
        JTokenType.None => Kata::AbstractCreateClause.Types.Text,
        JTokenType.Null => Kata::AbstractCreateClause.Types.Text,
        JTokenType.Object => Kata::AbstractCreateClause.Types.Text,
        JTokenType.String => Kata::AbstractCreateClause.Types.Text,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
