using System.Security.Claims;
using ACorp.Application;
using ACorp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACorp.Pages.Dashboard;

[Authorize]
public class LogoutModel : PageModel
{
    private readonly ILogger<LogoutModel> _logger;
    private readonly ApplicationDbContext _db;
    private readonly AuthService _authService;

    public LogoutModel(ILogger<LogoutModel> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
        _authService = new(_db);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _authService.LogoutAsync(HttpContext);
        return RedirectToPage("/index");
    }
}
