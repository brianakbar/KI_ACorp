using System.Security.Claims;
using System.Text.RegularExpressions;
using ACorp.Application;
using ACorp.Data;
using ACorp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace ACorp.Pages.Dashboard;

[Authorize]
public class AccountModel : PageModel
{
    public string? Name { get; set; } = "";
    public string? Email { get; set; } = "";
    public string? PhoneNumber { get; set; } = "";
    public string? NIK { get; set; } = "";
    public string? Key { get; set; } = "";

    private readonly ILogger<AccountModel> _logger;
    private readonly ApplicationDbContext _db;

    public AccountModel(ILogger<AccountModel> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public void OnGet()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        Name = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
        Email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        var user = _db.Users.FirstOrDefault(u => u.Email == Email);
        NIK = CryptoService.DecryptAESFromString(user?.AesNik!);
        PhoneNumber = CryptoService.DecryptDESFromString(user?.DesPhoneNumber!);
        Key = user?.SymmetricKey;

        // var user = _db.Users.FirstOrDefault(u => u.Email == Email);
        // PhoneNumber = _cryptoService.DecryptAESFromString(user.AesPhoneNumber);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // generate symmetric key
        var symmetricKeyByte = Cryptography.KeyParameterGeneration(128);
        var symmetricKeyString = Convert.ToBase64String(symmetricKeyByte);

        // generate RSA keys
        var (publicKey, privateKey) = Cryptography.GenerateRSAKeys();

        // encrypt symmetric key with RSA public key
        var encryptedSymmetricKey = Cryptography.EncryptWithRSA(symmetricKeyString, publicKey);

        // store the publicKey, privateKey, and encryptedSymmetricKey to database
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        Email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        var user = _db.Users.FirstOrDefault(u => u.Email == Email);
        if (user == null)
        {
            return RedirectToPage("/Login");
        }
        user.SymmetricKey = symmetricKeyString;
        user.RSAPublicKey = publicKey;
        user.RSAPrivateKey = privateKey;
        await _db.SaveChangesAsync();
        Key = encryptedSymmetricKey;

        return RedirectToPage("/Dashboard/Account");
    }
}
