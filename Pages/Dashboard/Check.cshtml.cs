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
    private readonly RequestDataService _requestDataService;

    public CheckModel(ApplicationDbContext db, ILogger<CheckModel> logger)
    {
        _db = db;
        _logger = logger;
        _authService = new AuthService(_db);
        _requestDataService = new RequestDataService(_db);
    }

    public void OnGet()
    {

    }

    public async Task OnPostRequestCheckAsync(string email)
    {
        User? UserToRequest = await _authService.FindUserAsync(email);

        if (UserToRequest == null) return;

        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var identityEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

        if (UserToRequest.Email == identityEmail) return;

        User? requester = await _authService.FindUserAsync(identityEmail ?? "");

        if (requester == null) return;

        _requestDataService.Request(requester, UserToRequest);
    }
}