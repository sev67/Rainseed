using System.Security.Cryptography;
using System.Text;

namespace StandardLib.Singletons;

public static class Sha256
{
    public static string Hash(in string rawData)
    {
        using SHA256 sha256Hash = SHA256.Create();
        
        string result;
        {
            byte[] binary = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            result = Convert.ToBase64String(binary).TrimEnd('=');
        }
        
        result = result.Replace('+', '-');
        result = result.Replace('/', '_');
        return result;
    }  
}