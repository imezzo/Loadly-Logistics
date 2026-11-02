using System.Security.Cryptography;
using System.Text;
namespace LoadlyAggregator.Domain.Common.Utilities;



public static class CodeGenerator
{
    private const string AlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";





    public static string GenerateAlphanumericCode(int length = 6)
    {
        var code = new StringBuilder(length);
        var randomBytes = new byte[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        for (int i = 0; i < length; i++)
        {
            code.Append(AlphanumericChars[randomBytes[i] % AlphanumericChars.Length]);
        }
        return code.ToString();
    }





    public static string GenerateNumericCode(int length = 6)
    {
        var min = (int)Math.Pow(10, length - 1);
        var max = (int)Math.Pow(10, length) - 1;
        return RandomNumberGenerator.GetInt32(min, max + 1).ToString();
    }
}
