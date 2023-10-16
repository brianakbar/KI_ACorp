using System.Security.Claims;
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

    public AccountModel(ILogger<AccountModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        Name = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
        Email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        PhoneNumber = claimsIdentity?.FindFirst(ClaimTypes.MobilePhone)?.Value;
    }
}
