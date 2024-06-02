using Challengify.Entities.Database;
using Challengify.Entities.Models;
using Challengify.Entities.Models.DataTransferObject.Response;
using Challengify.Services;
using Microsoft.EntityFrameworkCore;

public class UserService : IUserService
{
    private readonly IAppDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public UserService(IAppDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        // check if user with the same name and email already exists
        if (await _dbContext.Users.AnyAsync(u => u.Email == user.Email) || await _dbContext.Users.AnyAsync(u => u.Name == user.Name))
        {
            throw new InvalidOperationException("User with the same email or name already exists");
        }

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<UserResponseDto> GetUserResponseDtoAsync(int userId)
    {
        User user = await GetUserAsync(userId);
        return new UserResponseDto(user);
    }

    public async Task<User> DeleteUserAsync(int userId)
    {
        User user = await _dbContext.Users.FindAsync(userId) ?? throw new KeyNotFoundException("User not found");
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> GetUserAsync(int userId)
    {
        string cacheKey = $"user_{userId}";
        User? user = await _cacheService.GetObjectAsync<User>(cacheKey);

        if (user == null)
        {
            user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId)
                   ?? throw new KeyNotFoundException("User not found");
            await _cacheService.SetObjectAsync(cacheKey, user, TimeSpan.FromMinutes(5));
        }

        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        User user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email) ?? throw new KeyNotFoundException("User not found");
        return user;
    }



    public async Task<User> UpdateUserAsync(User user)
    {
        User existingUser = await _dbContext.Users.FindAsync(user.UserId) ?? throw new KeyNotFoundException("User not found");
        existingUser.Update(user);

        await _cacheService.RemoveAsync($"user_{user.UserId}");

        await _dbContext.SaveChangesAsync();
        return existingUser;
    }
}