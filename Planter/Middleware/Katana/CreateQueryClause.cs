using Newtonsoft.Json.Linq;
using Planter.Factories.JObject;

namespace Planter.Middleware.Katana;

public class CreateQueryClause : Kata::CreateQueryClause
{
    public CreateQueryClause(Newtonsoft.Json.Linq.JObject structure)
    {
        Name = TableFactory.TableName(structure["dgraph.type"]?.Value<string>() ?? Guid.NewGuid().ToString());
        Columns = BuildColumns(new TableFactory(structure));
    }

    virtual protected Column GetUidColumn()
        => new ("id", Types.AutoId, true, true);

    private List<Column> BuildColumns(JObject structure)
    {
        Std::Vector<Column> columns = new ();

        columns += GetUidColumn();
        
        foreach (JProperty predicate in structure.Properties())
            columns += new Planter.Middleware.Katana.Column(predicate);

        return columns;
    }
}