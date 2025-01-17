using System;
using System.Collections.Generic;
using CashFlo.Model;

namespace CashFlo.Services.Interface
{
    public interface IUserServices
    {
        bool Login(User user);
        bool Register(User user);
        bool DeleteUser(string username);
        string GetLoggedInUsername();  // Changed from Task<string> to string
        bool UpdatePrimaryBalance( decimal amount);  // Changed from Task<bool> to bool
        bool Logout();  // Changed from Task<bool> to bool
        string GetUserCurrency();  // Changed from Task<string> to string

        // Removed async version of SetUserCurrencyAsync
    }
}
