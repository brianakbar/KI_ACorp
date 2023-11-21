using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ACorp.Application;
using ACorp.Data;
using ACorp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ACorp.Pages;

[Authorize]
public class CheckModel : PageModel
{

    private readonly ILogger<CheckModel> _logger;
    private readonly ApplicationDbContext _db;
    private readonly AuthService _authService;

    public CheckModel(ApplicationDbContext db, ILogger<CheckModel> logger)
    {
        _db = db;
        _logger = logger;
        _authService = new AuthService(_db);
    }

    public void OnGet()
    {

    }

    public async void OnPostRequestCheckAsync()
    {
        string email = Request.Form["User"].ToString();

        User? UserToRequest = await _authService.FindUserAsync(email);

        if (UserToRequest == null) return;

        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var identityEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

        if (UserToRequest.Email == identityEmail) return;
    }
}