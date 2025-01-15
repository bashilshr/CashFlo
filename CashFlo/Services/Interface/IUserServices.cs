using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CashFlo.Model;

namespace CashFlo.Services.Interface
{
    public interface IUserServices
    {
        bool Login(User user);
        bool Register(User user);
        bool DeleteUser(string username);
        List<User> GetAllUsers();
        Task<string> GetLoggedInUsernameAsync();
        Task<decimal> GetPrimaryBalanceAsync(string username); // Method to get the user's primary balance
        Task<bool> UpdatePrimaryBalanceAsync(string username, int amount, bool isCredit); // Method to update the user's primary balance
        Task<bool> LogoutAsync();
        Task<string> GetUserCurrencyAsync();
        
        //Task<bool> SetUserCurrencyAsync(Currency currency);
    }
}
