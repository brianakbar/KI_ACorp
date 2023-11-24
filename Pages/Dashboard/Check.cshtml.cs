using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using ACorp.Application;
using ACorp.Data;
using ACorp.Models;
using ACorp.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ACorp.Pages;

[Authorize]
public class CheckModel : PageModel
{
    [BindProperty(Name = "key")]
    public string Key { get; set; } = "";

    public IEnumerable<User> RequestedUsers { get; set; } = new List<User>();

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CheckModel> _logger;
    private readonly ApplicationDbContext _db;
    private readonly AuthService _authService;
    private readonly RequestDataService _requestDataService;
    private readonly DocumentService _documentService;

    public CheckModel(IWebHostEnvironment environment, ApplicationDbContext db, ILogger<CheckModel> logger)
    {
        _environment = environment;
        _db = db;
        _logger = logger;
        _authService = new AuthService(_db);
        _requestDataService = new RequestDataService(_db);
        _documentService = new DocumentService(_environment, _db);
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

        await _requestDataService.Request(requester, UserToRequest);

        return RedirectToPage("/dashboard/check");
    }

    public async Task<IActionResult> OnPostKtpAsync(int id, string key)
    {
        _logger.LogInformation("key: " + key);

        User? user = await GetCurrentUserAsync();
        if (user == null) return NotFound();

        User? requestedUser = await _authService.FindUserAsync(id);
        if (requestedUser == null) return NotFound();

        if (user.RSAPrivateKey == null) throw new ApplicationException("User with id: " + user.Id + " doesn't have private key");
        var docKey = Cryptography.DecryptWithRSA(Key, user.RSAPrivateKey);
        var (decryptedFile, fileName) = await _documentService.DownloadAsync(requestedUser, docKey, DocumentType.KTP);

        return File(decryptedFile, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> OnPostCvAsync(int id, string key)
    {
        User? user = await GetCurrentUserAsync();
        if (user == null) return NotFound();

        User? requestedUser = await _authService.FindUserAsync(id);
        if (requestedUser == null) return NotFound();

        if (user.RSAPrivateKey == null) throw new ApplicationException("User with id: " + user.Id + " doesn't have private key");
        var docKey = Cryptography.DecryptWithRSA(Key, user.RSAPrivateKey);
        var (decryptedFile, fileName) = await _documentService.DownloadAsync(requestedUser, docKey, DocumentType.CV);

        return File(decryptedFile, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> OnPostVideoAsync(int id, string key)
    {
        User? user = await GetCurrentUserAsync();
        if (user == null) return NotFound();

        User? requestedUser = await _authService.FindUserAsync(id);
        if (requestedUser == null) return NotFound();

        if (user.RSAPrivateKey == null) throw new ApplicationException("User with id: " + user.Id + " doesn't have private key");
        var docKey = Cryptography.DecryptWithRSA(Key, user.RSAPrivateKey);
        var (decryptedFile, fileName) = await _documentService.DownloadAsync(requestedUser, docKey, DocumentType.Video);

        return File(decryptedFile, "application/octet-stream", fileName);
    }

    async Task<User?> GetCurrentUserAsync()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null) return null;
        return await _authService.FindUserAsync(userEmail);
    }
}