using System;
using System.Collections.Generic;
using System.Linq;

namespace Katana.Compilers
{
    public class PostgresCompiler : Compiler
    {
        public PostgresCompiler()
        {
            LastId = "SELECT lastval() AS id";
        }

        public override string EngineCode { get; } = EngineCodes.PostgreSql;

        protected override string TranslateColumnType(AbstractCreateClause.Types columnType)
        {
            switch (columnType)
            {
                case AbstractCreateClause.Types.AutoId:
                    return "serial PRIMARY KEY";
                case AbstractCreateClause.Types.Text:
                    return "text";
                case AbstractCreateClause.Types.Decimal:
                    return "real";
                case AbstractCreateClause.Types.Number:
                    return "bigint";
                case AbstractCreateClause.Types.DateTime:
                    return "timestamp";
                case AbstractCreateClause.Types.Bool:
                    return "boolean";
                default:
                    throw new ArgumentOutOfRangeException(nameof(columnType), columnType, null);
            }
        }


        protected override string CompileBasicStringCondition(SqlResult ctx, BasicStringCondition x)
        {

            var column = Wrap(x.Column);

            var value = Resolve(ctx, x.Value) as string;

            if (value == null)
            {
                throw new ArgumentException("Expecting a non nullable string");
            }

            var method = x.Operator;

            if (new[] { "starts", "ends", "contains", "like", "ilike" }.Contains(x.Operator))
            {
                method = x.CaseSensitive ? "LIKE" : "ILIKE";

                switch (x.Operator)
                {
                    case "starts":
                        value = $"{value}%";
                        break;
                    case "ends":
                        value = $"%{value}";
                        break;
                    case "contains":
                        value = $"%{value}%";
                        break;
                }
            }

            string sql;

            if (x.Value is UnsafeLiteral)
            {
                sql = $"{column} {checkOperator(method)} {value}";
            }
            else
            {
                sql = $"{column} {checkOperator(method)} {Parameter(ctx, value)}";
            }

            if (!string.IsNullOrEmpty(x.EscapeCharacter))
            {
                sql = $"{sql} ESCAPE '{x.EscapeCharacter}'";
            }

            return x.IsNot ? $"NOT ({sql})" : sql;

        }

        override protected SqlResult CompileCreateQuery(Query query)
        {
            var ctx = new SqlResult
            {
                Query = query
            };

            CreateQueryClause create = ctx.Query.GetOneComponent<CreateQueryClause>("create", EngineCode);

            List<string> columns = new List<string>();
            foreach (AbstractCreateClause.Column column in create.Columns)
            {
                string columnQueryPart = $"{column.Name} {TranslateColumnType(column.Type)}";
                if (column.NotNull) columnQueryPart += " NOT NULL";
                if (column.Unique) columnQueryPart += " UNIQUE";

                columns.Add(columnQueryPart);
            }

            string table = Wrap(create.Name);
            ctx.RawSql = $"CREATE TABLE {table} ({string.Join(", ", columns)}); ";
            return ctx;
        }


        protected override string CompileBasicDateCondition(SqlResult ctx, BasicDateCondition condition)
        {
            var column = Wrap(condition.Column);

            string left;

            if (condition.Part == "time")
            {
                left = $"{column}::time";
            }
            else if (condition.Part == "date")
            {
                left = $"{column}::date";
            }
            else
            {
                left = $"DATE_PART('{condition.Part.ToUpperInvariant()}', {column})";
            }

            var sql = $"{left} {condition.Operator} {Parameter(ctx, condition.Value)}";

            if (condition.IsNot)
            {
                return $"NOT ({sql})";
            }

            return sql;
        }
    }
}
