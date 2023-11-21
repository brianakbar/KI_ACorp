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
    public IEnumerable<User> RequestedUsers { get; set; } = new List<User>();

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

    public async Task OnGetAsync()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var identityEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

        User? requester = await _authService.FindUserAsync(identityEmail ?? "");

        if (requester == null) return;

        RequestedUsers = await _requestDataService.GetAllRequestedUserAsync(requester);
    }

    public async Task<IActionResult> OnPostRequestCheckAsync(string email)
    {
        User? UserToRequest = await _authService.FindUserAsync(email);

        if (UserToRequest == null) return RedirectToPage("/dashboard/check");

        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var identityEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;

        if (UserToRequest.Email == identityEmail) return RedirectToPage("/dashboard/check");

        User? requester = await _authService.FindUserAsync(identityEmail ?? "");

        if (requester == null) return RedirectToPage("/dashboard/check");

        _requestDataService.Request(requester, UserToRequest);

        return RedirectToPage("/dashboard/check");
    }
}