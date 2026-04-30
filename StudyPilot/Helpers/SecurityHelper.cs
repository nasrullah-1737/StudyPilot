using System.Security.Cryptography;
using System.Text;

namespace StudyPilot.Helpers;

public static class SecurityHelper
{
    public static string HashPassword(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
