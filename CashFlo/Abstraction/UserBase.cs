using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CashFlo.Model;
namespace CashFlo.Abstraction;
public abstract class UserBase
{
    protected static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "users.json");

    protected List<User> LoadUsers()
    {
        if (!File.Exists(FilePath)) return new List<User>();
        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    protected void SaveUsers(List<User> users)
    {
        var json = JsonSerializer.Serialize(users);
        File.WriteAllText(FilePath, json);
    }
}
