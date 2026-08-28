using AssetPortal.Web.Helpers;
using AssetPortal.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetPortal.Web.Repositories;

public class UserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public User? FindByUsername(string username)
    {
        return _db.Users.FirstOrDefault(u => u.Username == username);
    }

    public User? FindByEmail(string email)
    {
        return _db.Users.FirstOrDefault(u => u.Email == email);
    }

    public bool TryAuthenticate(string username, string password, out User? user)
    {
        var found = FindByUsername(username);
        if (found is null || !PasswordHasher.Verify(password, found.PasswordHash))
        {
            user = null;
            return false;
        }
        user = found;
        return true;
    }

    public async Task<User> CreateAsync(string username, string fullName, string email, string password, string department)
    {
        var user = new User
        {
            Username = username,
            FullName = fullName,
            Email = email,
            PasswordHash = PasswordHasher.Hash(password),
            Department = department,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public User? Get(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }
}
