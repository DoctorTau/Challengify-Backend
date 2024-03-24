using System.Security.Cryptography;
using System.Text;

namespace Services.Utils;

public class ChallengeCodeGenerator : IChallengeCodeGenerator
{
    public string GenerateJoinCode(int challengeId, DateTime creationDate)
    {
        string combined = $"{challengeId}-{creationDate:yyyyMMddHHmmss}";

        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(combined));

            var hashPart = BitConverter.ToInt64(bytes, 0);

            hashPart = Math.Abs(hashPart);

            string encoded = ConvertToBase36(hashPart);

            return encoded.Length > 10 ? encoded.Substring(0, 10) : encoded.PadRight(10, '0');
        }
    }

    private static string ConvertToBase36(long number)
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (number == 0) return "0";
        var base36 = new StringBuilder();
        while (number > 0)
        {
            base36.Insert(0, chars[(int)(number % 36)]);
            number /= 36;
        }
        return base36.ToString();
    }
}
