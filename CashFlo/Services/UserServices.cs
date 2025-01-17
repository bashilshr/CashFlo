using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Newtonsoft.Json;
using System.Security.Cryptography;
using CashFlo.Model;
using CashFlo.Abstraction;
using CashFlo.Services.Interface;

public class UserServices : UserBase, IUserServices
{
    private User _user;
    protected static readonly string UserFilePath = Path.Combine(FileSystem.AppDataDirectory, "user.json");

    public const string SeedUsername = "admin";
    public const string SeedPassword = "password";

    public UserServices()
    {
        _user = LoadUser();

        // Seed admin if no user exists
        if (_user == null)
        {
            var salt = GenerateSalt();
            var hashedPassword = HashPassword(SeedPassword, salt);

            _user = new User
            {
                Username = SeedUsername,
                Password = hashedPassword,
                Salt = Convert.ToBase64String(salt),
                PrimaryBalance = 1000 // Default initial balance for admin
            };
            SaveUser(_user);
        }
    }

    // Implementing DeleteUser(string) as per the interface
    public bool DeleteUser(string username)
    {
        if (_user == null || _user.Username != username)
            return false;

        _user = null;
        File.Delete(UserFilePath); // Delete the user file
        return true;
    }


    public bool Login(User user)
    {
        if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            return false;

        if (_user == null || _user.Username != user.Username) return false;

        var salt = Convert.FromBase64String(_user.Salt);
        var hashedPassword = HashPassword(user.Password, salt);

        return _user.Password == hashedPassword;
    }

    public bool Register(User user)
    {
        if (_user != null && _user.Username == user.Username)
            return false;

        var salt = GenerateSalt();
        var hashedPassword = HashPassword(user.Password, salt);

        _user = new User
        {
            Username = user.Username,
            Password = hashedPassword,
            Salt = Convert.ToBase64String(salt),
            PrimaryBalance = 0 // Default balance for new user
        };
        SaveUser(_user);
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

    public string GetLoggedInUsername()
    {
        return _user?.Username ?? "Guest"; // Return the username of the logged-in user or default to "Guest"
    }



    // Update the balance of the single user's account
    public bool UpdatePrimaryBalance(decimal newBalance) 
    {
        if (_user == null) return false;

        _user.PrimaryBalance = newBalance;
        SaveUser(_user);
        return true;
    }

    public bool Logout()
    {
        return true; // Logout logic handled elsewhere
    }

    public string GetUserCurrency()
    {
        return "en-US"; // Default currency; adjust as needed
    }

    // Load the single user from file (deserialization)
    private User LoadUser()
    {
        if (!File.Exists(UserFilePath))
        {
            return null; // If no file exists, return null
        }

        var json = File.ReadAllText(UserFilePath);
        return JsonConvert.DeserializeObject<User>(json);
    }

    // Save the single user to file (serialization)
    private void SaveUser(User user)
    {
        var json = JsonConvert.SerializeObject(user, Formatting.Indented);
        File.WriteAllText(UserFilePath, json);
    }

}
