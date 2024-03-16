using System.Security.Cryptography;

namespace Challengify.Services;

public class PasswordService : IPasswordService
{
    public void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));

        using (var hmac = new HMACSHA512())
        {
            passwordSalt = Convert.ToBase64String(hmac.Key);
            passwordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        }
    }

    public bool VerifyPasswordHash(string password, string storedHashBase64, string storedSaltBase64)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(storedHashBase64)) throw new ArgumentException("Invalid stored hash.", nameof(storedHashBase64));
        if (string.IsNullOrWhiteSpace(storedSaltBase64)) throw new ArgumentException("Invalid stored salt.", nameof(storedSaltBase64));

        var storedHash = Convert.FromBase64String(storedHashBase64);
        var storedSalt = Convert.FromBase64String(storedSaltBase64);

        using (var hmac = new HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i]) return false;
            }
        }

        return true;
    }
}