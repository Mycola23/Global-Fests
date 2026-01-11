using GlobalFests.EFModels;
using GlobalFests.Repositories;
using System.Security.Cryptography;
using System.Text;
using static GlobalFests.Repositories.UserRepository;

namespace GlobalFests.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<List<User>> GetAllUsersAsync();
        Task<User> RegisterUserAsync(string username, string email, string password, int roleId, int? countryId = null);
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> VerifyUserAsync(int userId);

        public string GenerateSalt();
        public string HashPassword(string password, string salt);

        public bool VerifyPassword(string inputPassword, string passwordHash, string salt);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepository;

        public UserService(IUserRepo userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> RegisterUserAsync(string username, string email, string password, int roleId, int? countryId = null)
        {
            var existingUser = await GetUserByEmailAsync(email);
            if (existingUser != null)
                throw new InvalidOperationException("User with this email already exists");

            var salt = GenerateSalt();
            var passwordHash = HashPassword(password, salt);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Salt = salt,
                RoleId = roleId,
                CountryId = countryId,
                Verified = false,
                CreatedAt = DateTime.Now
            };

            return await _userRepository.CreateAsync(user);
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
                return null;

            var passwordHash = HashPassword(password, user.Salt);
            if (passwordHash != user.PasswordHash)
                return null;

            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> VerifyUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.Verified = true;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public string GenerateSalt()
        {
            var saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var combined = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, combined, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combined, saltBytes.Length, passwordBytes.Length);

            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(combined);
                return Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPassword(string inputPassword,string passwordHash, string salt)
        {
            var inputPasswordHash = HashPassword(inputPassword, salt);
            if (passwordHash == inputPasswordHash)
            {
                return true;
            }
                return false;
        }
    }
}
