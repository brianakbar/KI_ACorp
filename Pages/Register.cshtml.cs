using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACorp.Pages;

public class RegisterModel : PageModel
{
    public string? Email { get; set; } = null;

    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(ILogger<RegisterModel> logger)
    {
        _logger = logger;
    }

    public void OnPostEmail(string? inputEmail) {
        Email = inputEmail;
    }

    public void OnGet()
    {
    }
}

