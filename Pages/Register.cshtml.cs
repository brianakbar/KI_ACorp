using System.ComponentModel.DataAnnotations;
using System.Text;
using ACorp.Application;
using ACorp.Data;
using ACorp.Models;
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

    public RegisterModel(ApplicationDbContext db, ILogger<RegisterModel> logger)
    {
        _db = db;
        _logger = logger;
        _authService = new AuthService(_db);
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
            AesPassword = CryptoService.EncryptAESFromString(RegisterForm.Password),
            DesPassword = CryptoService.EncryptDESFromString(RegisterForm.Password),
            Rc4Password = CryptoService.EncryptRC4FromString(RegisterForm.Password),
            AesPhoneNumber = CryptoService.EncryptAESFromString(RegisterForm.PhoneNumber),
            DesPhoneNumber = CryptoService.EncryptDESFromString(RegisterForm.PhoneNumber),
            Rc4PhoneNumber = CryptoService.EncryptRC4FromString(RegisterForm.PhoneNumber),
            AesNik = CryptoService.EncryptAESFromString(RegisterForm.NIK),
            DesNik = CryptoService.EncryptDESFromString(RegisterForm.NIK),
            Rc4Nik = CryptoService.EncryptRC4FromString(RegisterForm.NIK),
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
    public string NIK { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Compare("Password", ErrorMessage = "Confirm password doesn't match")]
    public string ConfirmPassword { get; set; } = "";
}

