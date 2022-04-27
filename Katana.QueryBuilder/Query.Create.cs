using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Katana
{
    public partial class Query
    {
        public Query Create(string tableName, IEnumerable<AbstractCreateClause.Column> columns)
        {
            var columnsList = columns?.ToList();

            if ((columnsList?.Count ?? 0) == 0 | string.IsNullOrWhiteSpace(tableName))
            {
                throw new InvalidOperationException($"{nameof(columns)} and {nameof(tableName)} cannot be null or empty");
            }

            Method = "create";

            ClearComponent("create").AddComponent("create", new CreateQueryClause()
            {
                Name = tableName,
                Columns = columnsList
            });

            return this;
        }
    }
}
