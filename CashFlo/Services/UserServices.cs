using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography; // For salt generation
using System.Text;
using System.Threading.Tasks;
using CashFlo.Model;
using CashFlo.Services.Interface;
using CashFlo.Abstraction;
using Microsoft.AspNetCore.Cryptography.KeyDerivation; // For password hashing

namespace CashFlo.Services
{
    public class UserServices : UserBase, 
        IUserServices
    {
        private List<User> _users;

        public const string SeedUsername = "admin";
        public const string SeedPassword = "password";

        public UserServices()
        {
            _users = LoadUsers();

            if (!_users.Any())
            {
                var salt = GenerateSalt();
                var hashedPassword = HashPassword(SeedPassword, salt);

                _users.Add(new User
                {
                    Username = SeedUsername,
                    Password = hashedPassword,
                    Salt = Convert.ToBase64String(salt),
                    PrimaryBalance = 1000 // Default initial balance for admin
                });
                SaveUsers(_users);
            }
        }

        public bool DeleteUser(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return false;

            _users.Remove(user);
            SaveUsers(_users);
            return true;
        }

        public List<User> GetAllUsers() => _users;

        public bool Login(User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                return false;

            var existingUser = _users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser == null) return false;

            var salt = Convert.FromBase64String(existingUser.Salt);
            var hashedPassword = HashPassword(user.Password, salt);

            return existingUser.Password == hashedPassword;
        }

        public bool Register(User user)
        {
            if (_users.Any(u => u.Username == user.Username))
                return false;

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(user.Password, salt);

            _users.Add(new User
            {
                Username = user.Username,
                Password = hashedPassword,
                Salt = Convert.ToBase64String(salt),
                PrimaryBalance = 0 // Default balance for new users
            });
            SaveUsers(_users);
            return true;
        }

        private string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8]; // 16 bytes
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public async Task<string> GetLoggedInUsernameAsync()
        {
            return await Task.FromResult("LoggedInUsername");
        }

        public async Task<decimal> GetPrimaryBalanceAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                throw new InvalidOperationException("User not found.");
            return await Task.FromResult(user.PrimaryBalance);
        }

        public async Task<bool> UpdatePrimaryBalanceAsync(string username, int amount, bool isCredit)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return false;

            if (!isCredit && user.PrimaryBalance < amount)
                return false; // Insufficient balance

            user.PrimaryBalance += isCredit ? amount : -amount;
            SaveUsers(_users);
            return await Task.FromResult(true);
        }


        public async Task<bool> LogoutAsync()
        {
            return await Task.FromResult(true);
        }

        public async Task<string> GetUserCurrencyAsync()
        {
            return await Task.FromResult("en-US");
        }

    }
}
