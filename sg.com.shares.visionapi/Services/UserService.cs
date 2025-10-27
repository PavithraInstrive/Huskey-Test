using sg.com.shares.visionapi.IServices;
using sg.com.shares.visionapi.Models;

namespace sg.com.shares.visionapi.Services {
    public class UserService : IUserService {
        private readonly List<User> _users;

        public UserService() {
            // Simulating in-memory data store
            _users = new List<User>
            {
                new User { Id = 1, Name = "John Doe", Email = "john@example.com", IsActive = true, CreatedDate = DateTime.Now.AddDays(-30) },
                new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com", IsActive = true, CreatedDate = DateTime.Now.AddDays(-20) }
            };
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync() {
            return await Task.FromResult(_users);
        }

        public async Task<User> GetUserByIdAsync(int id) {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return await Task.FromResult(user);
        }

        public async Task<User> CreateUserAsync(User user) {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required", nameof(user.Email));

            user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            user.CreatedDate = DateTime.Now;
            _users.Add(user);
            return await Task.FromResult(user);
        }

        public async Task<bool> UpdateUserAsync(int id, User user) {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = _users.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
                return await Task.FromResult(false);

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.IsActive = user.IsActive;
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteUserAsync(int id) {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return await Task.FromResult(false);

            _users.Remove(user);
            return await Task.FromResult(true);
        }

        public async Task<bool> UserExistsAsync(int id) {
            return await Task.FromResult(_users.Any(u => u.Id == id));
        }
    }
}
