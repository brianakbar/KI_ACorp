using System.ComponentModel.DataAnnotations;
using System.Text;
using ACorp.Application;
using KiAcorp.Data;
using KiAcorp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ACorp.Pages;

public class RegisterModel : PageModel
{
    [BindProperty]
    public RegisterForm RegisterForm { get; set; }
    [BindProperty]
    public string? Email { get; set; } = null;

    private readonly ILogger<RegisterModel> _logger;
    private readonly ApplicationDbContext _db;
    private readonly AuthService _authService;
    private readonly CryptoService _cryptoService;

    public RegisterModel(ApplicationDbContext db, ILogger<RegisterModel> logger)
    {
        _db = db;
        _logger = logger;
        _authService = new AuthService(_db);
        _cryptoService = new CryptoService();
    }

    public void OnPostEmail()
    {
        Email = RegisterForm.Email;
    }

    public async Task<IActionResult> OnPostRegisterAsync(string email)
    {
        RegisterForm.Email = email;
        User newUser = new()
        {
            Fullname = RegisterForm.FullName,
            Email = RegisterForm.Email,
            AesPassword = _cryptoService.EncryptAESFromString(RegisterForm.Password),
            DesPassword = _cryptoService.EncryptDESFromString(RegisterForm.Password),
            Rc4Password = _cryptoService.EncryptRC4FromString(RegisterForm.Password),
            AesPhoneNumber = _cryptoService.EncryptAESFromString(RegisterForm.PhoneNumber),
            DesPhoneNumber = _cryptoService.EncryptDESFromString(RegisterForm.PhoneNumber),
            Rc4PhoneNumber = _cryptoService.EncryptRC4FromString(RegisterForm.PhoneNumber),
        };

        if (!await _authService.RegisterAsync(newUser)) return RedirectToPage("/register");
        if (!await _authService.LoginAsync(email, RegisterForm.Password, HttpContext)) return RedirectToPage("/index");

        return RedirectToPage("/dashboard/index");
    }

    public void OnGet()
    {
    }
}

public class RegisterForm
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = "";

    [Required]
    [StringLength(maximumLength: 200, MinimumLength = 5)]
    public string FullName { get; set; } = "";

    [Required]
    public string Gender { get; set; } = "";

    [Required]
    [DataType(DataType.PhoneNumber)]
    public string PhoneNumber { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Compare("Password", ErrorMessage = "Confirm password doesn't match")]
    public string ConfirmPassword { get; set; } = "";
}

