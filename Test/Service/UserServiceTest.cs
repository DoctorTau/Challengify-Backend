using Moq;
using Microsoft.EntityFrameworkCore;
using Challengify.Entities.Database;
using Challengify.Entities.Models;
using Challengify.Services;
using MockQueryable.Moq;

namespace Test.Service
{

    public class UserServiceTests
    {
        private readonly List<User> _users;
        private readonly Mock<DbSet<User>> _mockUserDbSet;
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly Mock<ICacheService> _mockCacheService;

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _users = [
                new User { UserId = 1, Name = "FirstUser", Email = "first@mail.com", PasswordHash = "hash", PasswordSalt = "salt" },
            new User { UserId = 2, Name = "SecondUser", Email = "second@mail.com", PasswordHash = "hash", PasswordSalt = "salt" },
            new User { UserId = 3, Name = "ThirdUser", Email = "third@mail.com", PasswordHash = "hash", PasswordSalt = "salt" } ];

            _mockUserDbSet = _users.AsQueryable().BuildMockDbSet();

            _mockDbContext = new Mock<IAppDbContext>();
            _mockDbContext.Setup(c => c.Users).Returns(_mockUserDbSet.Object);

            _mockCacheService = new Mock<ICacheService>();

            _userService = new UserService(_mockDbContext.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task CreateUserAsync_UserAlreadyExists_ThrowsInvalidOperationException()
        {
            var newUser = _users[0];

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () => await _userService.CreateUserAsync(newUser));
            Assert.Equal("User with the same email or name already exists", exception.Message);
        }

        [Fact]
        public async Task CreateUserAsync_UserDoesNotExist_AddsUser()
        {
            var newUser = new User { Name = "NewUser", Email = "new@example.com", PasswordHash = "hash", PasswordSalt = "salt" };

            // Act
            var createdUser = await _userService.CreateUserAsync(newUser);

            // Assert
            Assert.Equal(newUser, createdUser);
            _mockUserDbSet.Verify(m => m.AddAsync(It.Is<User>(u => u == newUser), default), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetUserResponseDtoAsync_UserExists_ReturnsUserResponseDto()
        {
            // Arrange 
            User existingUser = _users[0];

            // Act
            var userResponseDto = await _userService.GetUserResponseDtoAsync(1);

            // Assert
            Assert.Equal(existingUser.Name, userResponseDto.Name);
            Assert.Equal(existingUser.Email, userResponseDto.Email);
        }

        [Fact]
        public async Task GetUserAsync_UserExists_ReturnsUser()
        {
            // Arrange
            User existingUser = _users[0];

            // Act
            var user = await _userService.GetUserAsync(1);

            // Assert
            Assert.Equal(existingUser, user);
            _mockCacheService.Verify(m => m.SetObjectAsync($"user_{1}", existingUser, TimeSpan.FromMinutes(5)), Times.Once);
        }

        [Fact]
        public async Task GetUserAsync_UserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _userService.GetUserAsync(4));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task DeleteUserAsync_UserExists_RemovesUser()
        {
            // Arrange
            User existingUser = _users[0];

            // Act
            var deletedUser = await _userService.DeleteUserAsync(1);

            // Assert
            Assert.Equal(existingUser, deletedUser);
            _mockUserDbSet.Verify(m => m.Remove(It.Is<User>(u => u == existingUser)), Times.Once);
            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_UserExists_UpdatesUser()
        {
            // Arrange
            User existingUser = _users[0];
            User updatedUser = new() { UserId = 1, Name = "UpdatedUser", Email = "update@email.com", PasswordHash = "hash", PasswordSalt = "salt" };

            // Act
            var user = await _userService.UpdateUserAsync(updatedUser);

            // Assert
            Assert.Equal(updatedUser.Name, existingUser.Name);
            Assert.Equal(updatedUser.Email, existingUser.Email);
            Assert.Equal(updatedUser.PasswordHash, existingUser.PasswordHash);
            Assert.Equal(updatedUser.PasswordSalt, existingUser.PasswordSalt);

            _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_UserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            User updatedUser = new() { UserId = 4, Name = "UpdatedUser", Email = "update@email.com", PasswordHash = "hash", PasswordSalt = "salt" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _userService.UpdateUserAsync(updatedUser));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserExists_ReturnsUser()
        {
            // Arrange
            User existingUser = _users[0];

            // Act
            var user = await _userService.GetUserByEmailAsync("first@mail.com");

            // Assert
            Assert.Equal(existingUser, user);
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _userService.GetUserByEmailAsync("kekis@mail.com"));
            Assert.Equal("User not found", exception.Message);
        }
    }

}