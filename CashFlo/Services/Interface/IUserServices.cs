using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CashFlo.Model;

namespace CashFlo.Services.Interface;

public interface IUserServices
{

    bool Login(User user);

    bool Register(User user);

    bool DeleteUser(string username);
    List<User> GetAllUsers();
    Task<string> GetLoggedInUsernameAsync();

}
