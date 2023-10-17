using System.Security.Claims;
using ACorp.Application;
using KiAcorp.Data;
using KiAcorp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACorp.Pages.Dashboard;

[Authorize]
public class FileModel : PageModel
{
    private readonly ILogger<FileModel> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _db;
    private readonly AuthService _authService;
    private readonly DocumentService _documentService;

    public FileModel(ILogger<FileModel> logger, ApplicationDbContext db, IWebHostEnvironment environment)
    {
        _environment = environment;
        _db = db;
        _authService = new(_db);
        _documentService = new(_db, _environment);
        _logger = logger;
    }

    [BindProperty]
    public IFormFile Upload { get; set; }

    public Document? DocumentKTP { get; set; }
    public Document? DocumentCV { get; set; }
    public Document? DocumentVideo { get; set; }

    public async Task OnGetAsync()
    {
        User user = await GetUser() ?? throw new InvalidOperationException("User is not found.");
        DocumentKTP = await _documentService.GetDocumentByTypeAsync(user, DocumentType.KTP);
        DocumentCV = await _documentService.GetDocumentByTypeAsync(user, DocumentType.CV);
        DocumentVideo = await _documentService.GetDocumentByTypeAsync(user, DocumentType.Video);
    }

    public async Task<IActionResult> OnPostKtpAsync()
    {
        User user = await GetUser() ?? throw new InvalidOperationException("User is not found.");
        await _documentService.UploadDocumentAsync(
            user,
            Upload,
            user.Id + "_" + "KTP" + Path.GetExtension(Upload.FileName),
            DocumentType.KTP
        );
        return RedirectToPage("/Dashboard/File");
    }

    public async Task<IActionResult> OnPostCvAsync()
    {
        User user = await GetUser() ?? throw new InvalidOperationException("User is not found.");
        await _documentService.UploadDocumentAsync(
            user,
            Upload,
            user.Id + "_" + "CV" + Path.GetExtension(Upload.FileName),
            DocumentType.CV
        );
        return RedirectToPage("/Dashboard/File");
    }

    public async Task<IActionResult> OnPostVideoAsync()
    {
        User user = await GetUser() ?? throw new InvalidOperationException("User is not found.");
        await _documentService.UploadDocumentAsync(
            user,
            Upload,
            user.Id + "_" + "Video" + Path.GetExtension(Upload.FileName),
            DocumentType.Video
        );
        return RedirectToPage("/Dashboard/File");
    }

    private async Task<User?> GetUser()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        string email = (claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value) ?? throw new InvalidOperationException("User email is not found.");

        return await _authService.FindUserAsync(email);
    }
}
