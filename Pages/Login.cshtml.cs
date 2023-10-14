using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACorp.Pages;

public class LoginModel : PageModel
{
    const string AuthenticationType = "CookieAuth";

    [BindProperty]
    public Credential Credential { get; set; }

    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ILogger<LoginModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
        if (!ModelState.IsValid) return Page();

        if (Credential.Email == "user@gmail.com" && Credential.Password == "password")
        {
            List<Claim> claims = new() {
                new(ClaimTypes.Email, Credential.Email)
            };
            ClaimsIdentity identity = new(claims, AuthenticationType);
            ClaimsPrincipal principal = new(identity);

            await HttpContext.SignInAsync(AuthenticationType, principal);

            return RedirectToPage("/dashboard/index");
        }

        return Page();
    }
}

public class Credential
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

