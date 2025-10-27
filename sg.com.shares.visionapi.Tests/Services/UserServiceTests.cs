using sg.com.shares.visionapi.Models;
using sg.com.shares.visionapi.Services;

namespace sg.com.shares.visionapi.Tests {
    public class UserServiceTests {
        private readonly UserService _userService;

        public UserServiceTests() {
            _userService = new UserService();
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers() {
            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser() {
            // Arrange
            int userId = 1;

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("John Doe", result.Name);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull() {
            // Arrange
            int invalidUserId = 999;

            // Act
            var result = await _userService.GetUserByIdAsync(invalidUserId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_WithValidUser_ShouldCreateUser() {
            // Arrange
            var newUser = new User {
                Name = "Test User",
                Email = "test@example.com",
                IsActive = true
            };

            // Act
            var result = await _userService.CreateUserAsync(newUser);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Test User", result.Name);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task CreateUserAsync_WithNullUser_ShouldThrowArgumentNullException() {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _userService.CreateUserAsync(null));
        }

        [Fact]
        public async Task CreateUserAsync_WithEmptyEmail_ShouldThrowArgumentException() {
            // Arrange
            var invalidUser = new User {
                Name = "Test User",
                Email = "",
                IsActive = true
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.CreateUserAsync(invalidUser));
            Assert.Contains("Email is required", exception.Message);
        }

        [Fact]
        public async Task CreateUserAsync_WithNullEmail_ShouldThrowArgumentException() {
            // Arrange
            var invalidUser = new User {
                Name = "Test User",
                Email = null,
                IsActive = true
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _userService.CreateUserAsync(invalidUser));
            Assert.Contains("Email is required", exception.Message);
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidUser_ShouldUpdateUser() {
            // Arrange
            int userId = 1;
            var updatedUser = new User {
                Name = "Updated Name",
                Email = "updated@example.com",
                IsActive = false
            };

            // Act
            var result = await _userService.UpdateUserAsync(userId, updatedUser);

            // Assert
            Assert.True(result);
            var user = await _userService.GetUserByIdAsync(userId);
            Assert.Equal("Updated Name", user.Name);
            Assert.Equal("updated@example.com", user.Email);
            Assert.False(user.IsActive);
        }

        [Fact]
        public async Task UpdateUserAsync_WithInvalidId_ShouldReturnFalse() {
            // Arrange
            int invalidUserId = 999;
            var user = new User {
                Name = "Test User",
                Email = "test@example.com",
                IsActive = true
            };

            // Act
            var result = await _userService.UpdateUserAsync(invalidUserId, user);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateUserAsync_WithNullUser_ShouldThrowArgumentNullException() {
            // Arrange
            int userId = 1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _userService.UpdateUserAsync(userId, null));
        }

        [Fact]
        public async Task DeleteUserAsync_WithValidId_ShouldDeleteUser() {
            // Arrange
            int userId = 1;

            // Act
            var result = await _userService.DeleteUserAsync(userId);

            // Assert
            Assert.True(result);
            var deletedUser = await _userService.GetUserByIdAsync(userId);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task DeleteUserAsync_WithInvalidId_ShouldReturnFalse() {
            // Arrange
            int invalidUserId = 999;

            // Act
            var result = await _userService.DeleteUserAsync(invalidUserId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UserExistsAsync_WithValidId_ShouldReturnTrue() {
            // Arrange
            int userId = 1;

            // Act
            var result = await _userService.UserExistsAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserExistsAsync_WithInvalidId_ShouldReturnFalse() {
            // Arrange
            int invalidUserId = 999;

            // Act
            var result = await _userService.UserExistsAsync(invalidUserId);

            // Assert
            Assert.False(result);
        }
    }
}
