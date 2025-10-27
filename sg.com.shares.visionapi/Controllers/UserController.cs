using Microsoft.AspNetCore.Mvc;
using sg.com.shares.visionapi.IServices;
using sg.com.shares.visionapi.Models;

namespace sg.com.shares.visionapi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger) {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers() {
            var apiName = nameof(GetAllUsers);

            try {
                _logger.LogInformation("{Api} - Request received", apiName);

                var users = await _userService.GetAllUsersAsync();

                _logger.LogInformation("{Api} - Success | Count: {Count}", apiName, users.Count());
                return Ok(users);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "{Api} - Failure", apiName);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id) {
            var apiName = nameof(GetUser);

            try {
                _logger.LogInformation("{Api} - Request received | ID: {Id}", apiName, id);

                if (id <= 0) {
                    _logger.LogWarning("{Api} - Invalid user ID: {Id}", apiName, id);
                    return BadRequest("Invalid user ID");
                }
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) {
                    _logger.LogInformation("{Api} - User not found | ID: {Id}", apiName, id);
                    return NotFound($"User with ID {id} not found");
                }
                _logger.LogInformation("{Api} - Success | ID: {Id} | UserName: {UserName}", apiName, user.Id, user.Name);
                return Ok(user);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "{Api} - Failure | ID: {Id}", apiName, id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
            {
            var apiName = nameof(CreateUser);

            try
            {
                _logger.LogInformation("{Api} - Request received | User: {@User}", apiName, user);

                if (user == null)
                {
                    _logger.LogWarning("{Api} - Null user object", apiName);
                    return BadRequest("User object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("{Api} - Invalid model state", apiName);
                    return BadRequest(ModelState);
                }

                var createdUser = await _userService.CreateUserAsync(user);

                _logger.LogInformation("{Api} - Success | ID: {Id} | UserName: {UserName}", apiName, createdUser.Id, createdUser.Name);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "{Api} - Validation failed | User: {UserName}", apiName, user?.Name);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Api} - Failure | User: {UserName}", apiName, user?.Name);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] User user) {
            var apiName = nameof(UpdateUser);

            try {
                _logger.LogInformation("{Api} - Request received | ID: {Id} | User: {@User}", apiName, id, user);


                if (id <= 0) {
                    _logger.LogWarning("{Api} - Invalid user ID: {Id}", apiName, id);
                    return BadRequest("Invalid user ID");
                }

                if (user == null) {
                    _logger.LogWarning("{Api} - User object is null | ID: {Id}", apiName, id);
                    return BadRequest("User object is null");
                }

                if (!ModelState.IsValid) {
                    _logger.LogWarning("{Api} - Invalid model state", apiName);
                    return BadRequest(ModelState);
                }
                var updated = await _userService.UpdateUserAsync(id, user);
                if (!updated) {
                    _logger.LogInformation("{Api} - User not found | ID: {Id}", apiName, id);
                    return NotFound($"User with ID {id} not found");
                }

                _logger.LogInformation("{Api} - Success | Updated ID: {Id}", apiName, id);
                return NoContent();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "{Api} - Failure | ID: {Id}", apiName, id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id) {
            var apiName = nameof(DeleteUser);

            try {
                _logger.LogInformation("{Api} - Request received | ID: {Id}", apiName, id);

                if (id <= 0) {
                    _logger.LogWarning("{Api} - Invalid user ID: {Id}", apiName, id);
                    return BadRequest("Invalid user ID");
                }

                var deleted = await _userService.DeleteUserAsync(id);
                if (!deleted) {
                    _logger.LogInformation("{Api} - User not found | ID: {Id}", apiName, id);
                    return NotFound($"User with ID {id} not found");
                }
                _logger.LogInformation("{Api} - Success | Deleted ID: {Id}", apiName, id);
                return NoContent();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "{Api} - Failure | ID: {Id}", apiName, id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
