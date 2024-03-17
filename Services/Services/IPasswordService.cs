using Challengify.Entities.Models;

namespace Challengify.Services;

public interface IPasswordService
{
    void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt);
    bool VerifyPasswordHash(string password, string storedHash, string storedSalt);
    string CreateJwtToken(User user);
}