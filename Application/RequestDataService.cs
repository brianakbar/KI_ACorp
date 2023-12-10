using ACorp.Data;
using ACorp.Models;
using ACorp.Shared;
using Microsoft.EntityFrameworkCore;

namespace ACorp.Application;

public class RequestDataService
{
    readonly ApplicationDbContext _db;
    private AuthService _authService;

    public RequestDataService(ApplicationDbContext db)
    {
        _db = db;
        _authService = new AuthService(db);
    }

    public async Task Request(User requester, User requested)
    {
        if (IsRequestedByUser(requested, requester))
        {
            string key = await EncryptSymmetricKey(requester, requested);
            EmailService.SendEmail(requester, "Key to Decrypt " + requested.Fullname + " data", "Key: " + key);
            return;
        }

        string encryptedKey = await EncryptSymmetricKey(requester, requested);
        EmailService.SendEmail(requester, "Key to Decrypt " + requested.Fullname + " data", "Key: " + encryptedKey);

        _db.RequestData.Add(new RequestData()
        {
            RequesterId = requester.Id,
            RequestedId = requested.Id
        });

        _db.SaveChanges();
    }

    public async Task<IEnumerable<User>> GetAllRequestedUserAsync(User requester)
    {
        List<User> users = new();

        foreach (var data in _db.RequestData.ToList())
        {
            if (data.RequesterId != requester.Id) continue;

            User? user = await _authService.FindUserAsync(data.RequestedId);
            if (user == null) continue;

            users.Add(user);
        }

        return users;
    }

    public async Task<byte[]> GenerateDigitalSignatureAsync(User sender, byte[] buffer)
    {
        byte[] hashedBuffer = CryptoService.ComputeSHA256(buffer);
        if (sender.RSAPrivateKey == null) await CreateRSAKeys(sender);

        string privateKey = sender.RSAPrivateKey ?? throw new ApplicationException("User with id: " + sender.Id + " doesn't have RSA private key.");

        return Cryptography.SignWithRSA(hashedBuffer, privateKey);
    }

    public bool VerifyDigitalSignature(User sender, byte[] buffer, byte[] signature)
    {
        byte[] hashedBuffer = CryptoService.ComputeSHA256(buffer);

        string publicKey = sender.RSAPublicKey ?? throw new ApplicationException("User with id: " + sender.Id + " doesn't have RSA public key.");

        return Cryptography.VerifyWithRSA(hashedBuffer, signature, publicKey);
    }

    bool IsRequestedByUser(User requested, User requester)
    {
        foreach (var data in _db.RequestData.ToList())
        {
            if (data.RequesterId == requester.Id && data.RequestedId == requested.Id) return true;
        }

        return false;
    }

    async Task<string> EncryptSymmetricKey(User requester, User requested)
    {
        if (requested.SymmetricKey == null) throw new ApplicationException("User with id: " + requester.Id + " doesn't have symmetric key.");

        if (string.IsNullOrEmpty(requester.RSAPrivateKey) || string.IsNullOrEmpty(requester.RSAPublicKey))
        {
            await CreateRSAKeys(requester);
        }

        string? publicKey = requester.RSAPublicKey ?? throw new ApplicationException("User with id: " + requester.Id + " doesn't have RSA public key.");

        return Cryptography.EncryptWithRSA(requested.SymmetricKey, publicKey);
    }

    async Task CreateRSAKeys(User requester)
    {
        var (publicKey, privateKey) = Cryptography.GenerateRSAKeys();

        requester.RSAPrivateKey = privateKey;
        requester.RSAPublicKey = publicKey;

        _db.Users.Update(requester);

        await _db.SaveChangesAsync();
    }
}