using ACorp.Data;
using ACorp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using ACorp.Shared;
using System.Text;
using ACorp.Application;


namespace ACorp.Pages.Dashboard;

[Authorize]
public class DownloadModel : PageModel
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _db;
    private readonly AuthService _authService;
    private readonly DocumentService _documentService;

    public string? KTP { get; set; } = "";
    public string? CV { get; set; } = "";
    public string? Video { get; set; } = "";

    public DownloadModel(IWebHostEnvironment environment, ApplicationDbContext db)
    {
        _environment = environment;
        _db = db;
        _authService = new AuthService(_db);
        _documentService = new DocumentService(_environment, _db);
    }

    public void OnGet()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var userData = _db.Users.FirstOrDefault(u => u.Email == claimsIdentity!.FindFirst(ClaimTypes.Email)!.Value);
        var ktpDoc = userData != null ? _db.Documents.FirstOrDefault(d => d.UserId == userData.Id && d.Type == "ID_CARD") : null;
        KTP = ktpDoc?.Name + ktpDoc?.FileExtension;

        var cvDoc = userData != null ? _db.Documents.FirstOrDefault(d => d.UserId == userData.Id && d.Type == "CV") : null;
        CV = cvDoc?.Name + cvDoc?.FileExtension;

        var videoDoc = userData != null ? _db.Documents.FirstOrDefault(d => d.UserId == userData.Id && d.Type == "VIDEO") : null;
        Video = videoDoc?.Name + videoDoc?.FileExtension;
    }

    public async Task<IActionResult> OnPostKtpAsync()
    {
        User? user = await GetCurrentUserAsync();
        if (user == null) return NotFound();

        if (user.SymmetricKey == null) throw new ApplicationException("User with id: " + user.Id + " doesn't have symmetric key.");
        var (decryptedFile, fileName) = await _documentService.DownloadAsync(user, user.SymmetricKey, DocumentType.KTP);

        return File(decryptedFile, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> OnPostCv()
    {
        User? user = await GetCurrentUserAsync();
        if (user == null) return NotFound();

        if (user.SymmetricKey == null) throw new ApplicationException("User with id: " + user.Id + " doesn't have symmetric key.");
        var (decryptedFile, fileName) = await _documentService.DownloadAsync(user, user.SymmetricKey, DocumentType.CV);

        return File(decryptedFile, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> OnPostVideo()
    {
        User? user = await GetCurrentUserAsync();
        if (user == null) return NotFound();

        if (user.SymmetricKey == null) throw new ApplicationException("User with id: " + user.Id + " doesn't have symmetric key.");
        var (decryptedFile, fileName) = await _documentService.DownloadAsync(user, user.SymmetricKey, DocumentType.Video);

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