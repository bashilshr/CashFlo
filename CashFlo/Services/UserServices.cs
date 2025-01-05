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
    public class UserServices : UserBase, IUserServices
    {
        // Stores the list of users loaded from the file.
        private List<User> _users;

        // Default admin username and password for initial seeding.
        public const string SeedUsername = "admin";
        public const string SeedPassword = "password";

        // Constructor to initialize the user service.
        public UserServices()
        {
            // Load the list of users from the JSON file.
            _users = LoadUsers();

            // If no users are present, add a default admin user and save to the file.
            if (!_users.Any())
            {
                var salt = GenerateSalt();
                var hashedPassword = HashPassword(SeedPassword, salt);

                _users.Add(new User
                {
                    Username = SeedUsername,
                    Password = hashedPassword,
                    Salt = Convert.ToBase64String(salt) // Save the salt as Base64
                });
                SaveUsers(_users);
            }
        }

        // Deletes a user by username. Returns true if the user was deleted, false if not found.
        public bool DeleteUser(string username)
        {
            // Find the user with the specified username.
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null) // If no user is found, return false.
                return false;

            // Remove the user from the list and save the updated list to the file.
            _users.Remove(user);
            SaveUsers(_users);
            return true;
        }

        // Retrieves the list of all users.
        public List<User> GetAllUsers()
        {
            return _users; // Return the in-memory list of users.
        }

        // Logs in a user by checking if their username and password exist in the list.
        // Returns true if the user is authenticated, false otherwise.
        public bool Login(User user)
        {
            // Validate input for null or empty values.
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return false; // Invalid input.
            }

            // Find the user with the matching username.
            var existingUser = _users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser == null) return false; // User not found.

            // Hash the provided password using the stored salt.
            var salt = Convert.FromBase64String(existingUser.Salt);
            var hashedPassword = HashPassword(user.Password, salt);

            // Check if the hashed password matches the stored password.
            return existingUser.Password == hashedPassword;
        }

        // Registers a new user. Returns false if the username already exists, true if registration is successful.
        public bool Register(User user)
        {
            // Check if the username already exists in the list.
            if (_users.Any(u => u.Username == user.Username))
                return false; // Registration failed: user already exists.

            // Generate a new salt and hash the password.
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(user.Password, salt);

            // Add the new user to the list and save the updated list to the file.
            _users.Add(new User
            {
                Username = user.Username,
                Password = hashedPassword,
                Salt = Convert.ToBase64String(salt) // Save the salt
            });
            SaveUsers(_users);
            return true;
        }

        // Hashes a password using PBKDF2 with HMACSHA256.
        private string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        // Generates a random salt for password hashing.
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
            // Implementation to get the logged-in username
            return await Task.FromResult("LoggedInUsername");
        }

    }
}
