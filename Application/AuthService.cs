namespace ACorp.Authentication;

using System.Security.Claims;
using KiAcorp.Data;
using KiAcorp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AuthService
{
    const string AuthenticationType = "CookieAuth";

    readonly ApplicationDbContext _db;

    public AuthService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> RegisterAsync(User newUser)
    {
        if (await FindUserAsync(newUser.Email) != null) return false;

        await _db.Users.AddAsync(newUser);

        _db.SaveChanges();

        return true;
    }

    public async Task<bool> LoginAsync(string email, string password, HttpContext context)
    {
        User? foundUser = await FindUserAsync(email, password);

        if (foundUser != null)
        {
            List<Claim> claims = new() {
                new(ClaimTypes.Name, foundUser.Fullname),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.MobilePhone, foundUser.AesPhoneNumber ?? "")
            };
            ClaimsIdentity identity = new(claims, AuthenticationType);
            ClaimsPrincipal principal = new(identity);

            await context.SignInAsync(AuthenticationType, principal);

            return true;
        }

        return false;
    }

    public async Task<User?> FindUserAsync(string email)
    {
        User? foundUser = null;

        await _db.Users.ForEachAsync(user =>
        {
            if (user.Email == email) foundUser = user;
        });

        return foundUser;
    }

    public async Task<User?> FindUserAsync(string email, string password)
    {
        User? foundUser = await FindUserAsync(email);

        if (foundUser != null)
        {
            if (foundUser.AesPassword == password) return foundUser;
        }

        return null;
    }
}