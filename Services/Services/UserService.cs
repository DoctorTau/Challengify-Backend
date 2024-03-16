using Challengify.Models;
using Challengify.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> DeleteUserAsync(long userId)
    {
        User user = await _dbContext.Users.FindAsync(userId) ?? throw new KeyNotFoundException("User not found");
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> GetUserAsync(long userId)
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