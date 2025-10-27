using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using sg.com.shares.visionapi.Controllers;
using sg.com.shares.visionapi.IServices;
using sg.com.shares.visionapi.Models;


namespace sg.com.shares.visionapi.Tests {
    public class UserControllerTests {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _controller;

        public UserControllerTests() {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _controller = new UserController(_mockUserService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithNullService_ShouldThrowArgumentNullException() {
            var mockLogger = new Mock<ILogger<UserController>>();
            Assert.Throws<ArgumentNullException>(() => new UserController(null, mockLogger.Object));
        }

        #region GetAllUsers Tests

        //[Fact]
        //public async Task GetAllUsers_ShouldReturnOkWithUsers() {
        //    // Arrange
        //    var users = new List<User>
        //    {
        //        new User { Id = 1, Name = "User1", Email = "user1@test.com" },
        //        new User { Id = 2, Name = "User2", Email = "user2@test.com" }
        //    };
        //    _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

        //    // Act
        //    var result = await _controller.GetAllUsers();

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //    var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        //    Assert.Equal(2, returnedUsers.Count());
        //}

        //[Fact]
        //public async Task GetAllUsers_WhenExceptionThrown_ShouldReturnInternalServerError() {
        //    // Arrange
        //    _mockUserService.Setup(s => s.GetAllUsersAsync())
        //        .ThrowsAsync(new Exception("Database error"));

        //    // Act
        //    var result = await _controller.GetAllUsers();

        //    // Assert
        //    var statusResult = Assert.IsType<ObjectResult>(result.Result);
        //    Assert.Equal(500, statusResult.StatusCode);
        //}

        #endregion

        #region GetUser Tests

        //[Fact]
        //public async Task GetUser_WithValidId_ShouldReturnOkWithUser() {
        //    // Arrange
        //    var user = new User { Id = 1, Name = "Test User", Email = "test@test.com" };
        //    _mockUserService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);

        //    // Act
        //    var result = await _controller.GetUser(1);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //    var returnedUser = Assert.IsType<User>(okResult.Value);
        //    Assert.Equal(1, returnedUser.Id);
        //}

        //[Fact]
        //public async Task GetUser_WithInvalidId_ShouldReturnBadRequest() {
        //    // Act
        //    var result = await _controller.GetUser(0);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        //    Assert.Equal("Invalid user ID", badRequestResult.Value);
        //}

        //[Fact]
        //public async Task GetUser_WithNegativeId_ShouldReturnBadRequest() {
        //    // Act
        //    var result = await _controller.GetUser(-1);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        //    Assert.Equal("Invalid user ID", badRequestResult.Value);
        //}

        //[Fact]
        //public async Task GetUser_WhenUserNotFound_ShouldReturnNotFound() {
        //    // Arrange
        //    _mockUserService.Setup(s => s.GetUserByIdAsync(999)).ReturnsAsync((User)null);

        //    // Act
        //    var result = await _controller.GetUser(999);

        //    // Assert
        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        //    Assert.Contains("User with ID 999 not found", notFoundResult.Value.ToString());
        //}

        //[Fact]
        //public async Task GetUser_WhenExceptionThrown_ShouldReturnInternalServerError() {
        //    // Arrange
        //    _mockUserService.Setup(s => s.GetUserByIdAsync(It.IsAny<int>()))
        //        .ThrowsAsync(new Exception("Database error"));

        //    // Act
        //    var result = await _controller.GetUser(1);

        //    // Assert
        //    var statusResult = Assert.IsType<ObjectResult>(result.Result);
        //    Assert.Equal(500, statusResult.StatusCode);
        //}

        #endregion

        #region CreateUser Tests

        [Fact]
        public async Task CreateUser_WithValidUser_ShouldReturnCreatedAtAction() {
            // Arrange
            var newUser = new User { Name = "New User", Email = "new@test.com" };
            var createdUser = new User { Id = 3, Name = "New User", Email = "new@test.com" };
            _mockUserService.Setup(s => s.CreateUserAsync(newUser)).ReturnsAsync(createdUser);

            // Act
            var result = await _controller.CreateUser(newUser);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(_controller.GetUser), createdResult.ActionName);
            var returnedUser = Assert.IsType<User>(createdResult.Value);
            Assert.Equal(3, returnedUser.Id);
        }

        [Fact]
        public async Task CreateUser_WithNullUser_ShouldReturnBadRequest() {
            // Act
            var result = await _controller.CreateUser(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User object is null", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateUser_WithInvalidModelState_ShouldReturnBadRequest() {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");
            var user = new User { Email = "test@test.com" };

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateUser_WhenArgumentExceptionThrown_ShouldReturnBadRequest() {
            // Arrange
            var user = new User { Name = "Test", Email = "" };
            _mockUserService.Setup(s => s.CreateUserAsync(user))
                .ThrowsAsync(new ArgumentException("Email is required"));

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Email is required", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task CreateUser_WhenExceptionThrown_ShouldReturnInternalServerError() {
            // Arrange
            var user = new User { Name = "Test", Email = "test@test.com" };
            _mockUserService.Setup(s => s.CreateUserAsync(user))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region UpdateUser Tests

        //[Fact]
        //public async Task UpdateUser_WithValidUser_ShouldReturnNoContent() {
        //    // Arrange
        //    var user = new User { Id = 1, Name = "Updated User", Email = "updated@test.com" };
        //    _mockUserService.Setup(s => s.UpdateUserAsync(1, user)).ReturnsAsync(true);

        //    // Act
        //    var result = await _controller.UpdateUser(1, user);

        //    // Assert
        //    Assert.IsType<NoContentResult>(result);
        //}

        //[Fact]
        //public async Task UpdateUser_WithInvalidId_ShouldReturnBadRequest() {
        //    // Arrange
        //    var user = new User { Name = "Test", Email = "test@test.com" };

        //    // Act
        //    var result = await _controller.UpdateUser(0, user);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.Equal("Invalid user ID", badRequestResult.Value);
        //}

        //[Fact]
        //public async Task UpdateUser_WithNegativeId_ShouldReturnBadRequest() {
        //    // Arrange
        //    var user = new User { Name = "Test", Email = "test@test.com" };

        //    // Act
        //    var result = await _controller.UpdateUser(-1, user);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.Equal("Invalid user ID", badRequestResult.Value);
        //}

        //[Fact]
        //public async Task UpdateUser_WithNullUser_ShouldReturnBadRequest() {
        //    // Act
        //    var result = await _controller.UpdateUser(1, null);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.Equal("User object is null", badRequestResult.Value);
        //}

        //[Fact]
        //public async Task UpdateUser_WithInvalidModelState_ShouldReturnBadRequest() {
        //    // Arrange
        //    _controller.ModelState.AddModelError("Email", "Email is required");
        //    var user = new User { Name = "Test" };

        //    // Act
        //    var result = await _controller.UpdateUser(1, user);

        //    // Assert
        //    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //    Assert.IsType<SerializableError>(badRequestResult.Value);
        //}

        //[Fact]
        //public async Task UpdateUser_WhenUserNotFound_ShouldReturnNotFound() {
        //    // Arrange
        //    var user = new User { Name = "Test", Email = "test@test.com" };
        //    _mockUserService.Setup(s => s.UpdateUserAsync(999, user)).ReturnsAsync(false);

        //    // Act
        //    var result = await _controller.UpdateUser(999, user);

        //    // Assert
        //    var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        //    Assert.Contains("User with ID 999 not found", notFoundResult.Value.ToString());
        //}

        [Fact]
        public async Task UpdateUser_WhenExceptionThrown_ShouldReturnInternalServerError() {
            // Arrange
            var user = new User { Name = "Test", Email = "test@test.com" };
            _mockUserService.Setup(s => s.UpdateUserAsync(It.IsAny<int>(), user))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdateUser(1, user);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion

        #region DeleteUser Tests

        [Fact]
        public async Task DeleteUser_WithValidId_ShouldReturnNoContent() {
            // Arrange
            _mockUserService.Setup(s => s.DeleteUserAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_WithInvalidId_ShouldReturnBadRequest() {
            // Act
            var result = await _controller.DeleteUser(0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid user ID", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteUser_WithNegativeId_ShouldReturnBadRequest() {
            // Act
            var result = await _controller.DeleteUser(-1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid user ID", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteUser_WhenUserNotFound_ShouldReturnNotFound() {
            // Arrange
            _mockUserService.Setup(s => s.DeleteUserAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("User with ID 999 not found", notFoundResult.Value.ToString());
        }

        [Fact]
        public async Task DeleteUser_WhenExceptionThrown_ShouldReturnInternalServerError() {
            // Arrange
            _mockUserService.Setup(s => s.DeleteUserAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        #endregion
    }
}
