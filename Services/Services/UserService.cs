using Challengify.Entities.Database;
using Challengify.Entities.Models;
using Challengify.Services;

public class UserService : IUserService
{
    private readonly IAppDbContext _dbContext;

    public UserService(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
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
        User user = await _dbContext.Users.FindAsync(userId) ?? throw new KeyNotFoundException("User not found");
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        User existingUser = await _dbContext.Users.FindAsync(user.UserId) ?? throw new KeyNotFoundException("User not found");
        existingUser.Update(user);
        await _dbContext.SaveChangesAsync();
        return existingUser;
    }
}