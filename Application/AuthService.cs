namespace ACorp.Application;

using System.Security.Claims;
using ACorp.Data;
using ACorp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AuthService
{
    const string AuthenticationType = "CookieAuth";
    AuthCryptoType authCryptoType = AuthCryptoType.RC4;

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
                new(ClaimTypes.MobilePhone, GetPhoneNumber(foundUser) ?? "")
            };
            ClaimsIdentity identity = new(claims, AuthenticationType);
            ClaimsPrincipal principal = new(identity);

            await context.SignInAsync(AuthenticationType, principal);

            return true;
        }

        return false;
    }

    public async Task LogoutAsync(HttpContext context)
    {
        await context.SignOutAsync(AuthenticationType);
    }

    public async Task<User?> FindUserAsync(int id)
    {
        User? foundUser = null;

        await _db.Users.ForEachAsync(user =>
        {
            if (user.Id == id) foundUser = user;
        });

        return foundUser;
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
            if (ComparePassword(foundUser, password)) return foundUser;
        }

        return null;
    }

    bool ComparePassword(User user, string password)
    {
        if (authCryptoType == AuthCryptoType.AES)
        {
            return user.AesPassword == CryptoService.EncryptAESFromString(password);
        }
        else if (authCryptoType == AuthCryptoType.DES)
        {
            return user.DesPassword == CryptoService.EncryptDESFromString(password);
        }
        else
        {
            return user.Rc4Password == CryptoService.EncryptRC4FromString(password);
        }
    }

    string GetPhoneNumber(User user)
    {
        if (authCryptoType == AuthCryptoType.AES)
        {
            if (user.AesPhoneNumber == null) return "";
            return CryptoService.DecryptAESFromString(user.AesPhoneNumber);
        }
        else if (authCryptoType == AuthCryptoType.DES)
        {
            if (user.DesPhoneNumber == null) return "";
            return CryptoService.DecryptDESFromString(user.DesPhoneNumber);
        }
        else
        {
            if (user.Rc4PhoneNumber == null) return "";
            return CryptoService.DecryptRC4FromString(user.Rc4PhoneNumber);
        }
    }
}