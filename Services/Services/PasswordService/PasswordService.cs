using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Challengify.Entities.Models;
using Microsoft.IdentityModel.Tokens;

namespace Challengify.Services;

public class PasswordService : IPasswordService
{
    private static readonly string _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new ArgumentNullException("JWT_SECRET environment variable not set.");
    public string CreateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Status.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(10),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

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