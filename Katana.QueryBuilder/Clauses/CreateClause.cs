using System.Collections.Generic;

namespace Katana
{
    public abstract class AbstractCreateClause : AbstractClause
    {
        public enum Types
        {
            AutoId,
            Text,
            Bool,
            Decimal,
            Number,
            DateTime,
        }

        public class Column
        {
            public string Name { get; set; }
            public Types Type { get; set; }
            public bool NotNull { get; set; }
            public bool Unique { get; set; }

            /// <summary>
            /// Default constructor.
            /// </summary>
            public Column()
            {/* ... */}

            /// <summary>
            /// Parameterized constructor.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <param name="notNull"></param>
            /// <param name="unique"></param>
            public Column(string name, Types type, bool notNull = false, bool unique = false)
            {
                Name = name;
                Type = type;
                NotNull = notNull;
                Unique = unique;
            }
        }
    }

    public class CreateClause : AbstractCreateClause
    {
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public bool ReturnId { get; set; } = false;

        public override AbstractClause Clone()
        {
            return new CreateClause
            {
                Engine = Engine,
                Component = Component,
                Columns = Columns,
                Name = Name,
                ReturnId = ReturnId,
            };
        }
    }

    public class CreateQueryClause : AbstractCreateClause
    {
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public Query Query { get; set; }

        public override AbstractClause Clone()
        {
            return new CreateQueryClause
            {
                Engine = Engine,
                Component = Component,
                Columns = Columns,
                Name = Name,
                Query = Query.Clone(),
            };
        }
    }
}
