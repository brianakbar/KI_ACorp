using System.Security.Claims;
using ACorp.Application;
using ACorp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACorp.Pages.Dashboard;

[Authorize]
public class AccountModel : PageModel
{
    public string? Name { get; set; } = "";
    public string? Email { get; set; } = "";
    public string? PhoneNumber { get; set; } = "";

    private readonly ILogger<AccountModel> _logger;
    private readonly ApplicationDbContext _db;
    private readonly CryptoService _cryptoService;

    public AccountModel(ILogger<AccountModel> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
        _cryptoService = new CryptoService();
    }

    public void OnGet()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        Name = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
        Email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        PhoneNumber = claimsIdentity?.FindFirst(ClaimTypes.MobilePhone)?.Value;

        // var user = _db.Users.FirstOrDefault(u => u.Email == Email);
        // PhoneNumber = _cryptoService.DecryptAESFromString(user.AesPhoneNumber);
    }
}
