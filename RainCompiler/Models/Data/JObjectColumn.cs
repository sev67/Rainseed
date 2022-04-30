using Newtonsoft.Json.Linq;

namespace RainCompiler.Models.Data;

public class JObjectColumn : IEquatable<JObjectColumn>
{
    public JTokenType Type 
    { get; }
    
    public string Name 
    { get; }

    public JObjectColumn(JTokenType type, string name)
    {
        Type = type;
        Name = name;
    }
    
    public JObjectColumn(JProperty property)
    {
        Type = property.Type;
        Name = property.Name;
    }
    
    override public int GetHashCode() 
    => HashCode.Combine((int) Type, Name);
    
    public bool Equals(JObjectColumn? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type && Name == other.Name;
    }

    override public bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        
        return obj.GetType() == this.GetType() 
               && Equals((JObjectColumn) obj);
    } 

    public static bool operator ==(JObjectColumn self, JProperty property)
    => self.Name == property.Name && self.Type == property.Type;
    
    public static bool operator !=(JObjectColumn self, JProperty property)
    => self.Name != property.Name || self.Type != property.Type;

    public static explicit operator JObjectColumn(JProperty property)
    => new (property);
}