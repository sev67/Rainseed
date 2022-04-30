using System.Collections;
using Newtonsoft.Json.Linq;
using StandardLib.Types;

namespace RainCompiler.Models.Data;

public class JObjectCollection : IEnumerable<JObjectColumn>
{
    public Vector<JObjectColumn> Columns
    { get; set; } = new ();

    public JObjectColumn? this[string key]
    {
        get
        {
            foreach (JObjectColumn column in Columns)
            {
                if (column.Name != key) continue;
                return column;
            }

            return null;
        }
        set
        {
            if (value is null) throw new NullReferenceException("Assigned value must not be null.");
            
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Name != key) continue;
                
                Columns[i] = value;
                return;
            }

            Columns += value;
        }
    }

    public bool Contains(JProperty jprop)
    {
        foreach (JObjectColumn jObjectColumn in Columns)
        {
            if (jObjectColumn == jprop) return true;
        }

        return false;
    }

    public bool HasReferenceTypes()
    {
        foreach (JObjectColumn jObjectColumn in Columns)
        {
            if (jObjectColumn.Type == JTokenType.Array) return true;
        }

        return false;
    }

    public IEnumerator<JObjectColumn> GetEnumerator() 
    => Columns.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() 
    => GetEnumerator();
}