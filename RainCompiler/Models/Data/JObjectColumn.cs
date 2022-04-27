using Newtonsoft.Json.Linq;

namespace RainCompiler.Models.Data;

public class JObjectColumn
{
    public JTokenType Type 
    { get; set; }
    
    public string Name 
    { get; set; }
}