using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ACorp.Application;
using ACorp.Data;
using ACorp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ACorp.Pages;

public class LoginModel : PageModel
{
    const string AuthenticationType = "CookieAuth";

    [BindProperty]
    public Credential Credential { get; set; }

    private readonly ILogger<LoginModel> _logger;
    private readonly ApplicationDbContext _db;
    private readonly AuthService _authService;

    public LoginModel(ApplicationDbContext db, ILogger<LoginModel> logger)
    {
        _db = db;
        _logger = logger;
        _authService = new AuthService(_db);
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
        if (!ModelState.IsValid) return Page();

        if (await _authService.LoginAsync(Credential.Email, Credential.Password, HttpContext))
        {
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
