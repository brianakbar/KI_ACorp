using System.Security.Claims;
using ACorp.Authentication;
using KiAcorp.Data;
using KiAcorp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACorp.Pages.Dashboard;

[Authorize]
public class FileModel : PageModel
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _db;
    private readonly AuthService _authService;

    public FileModel(ApplicationDbContext db, IWebHostEnvironment environment)
    {
        _environment = environment;
        _db = db;
        _authService = new(_db);
    }

    [BindProperty]
    public IFormFile UploadKTP { get; set; }

    [BindProperty]
    public IFormFile UploadCV { get; set; }

    [BindProperty]
    public IFormFile UploadVideo { get; set; }

    public void OnGet()
    {

    }

    public async Task OnPostKtpAsync()
    {
        User user = await GetUser() ?? throw new InvalidOperationException("User is not found.");
        await UploadFile(UploadKTP, user.Id + "_" + "KTP" + Path.GetExtension(UploadKTP.FileName));
    }

    public async Task OnPostCvAsync()
    {
        User user = await GetUser() ?? throw new InvalidOperationException("User is not found.");
        await UploadFile(UploadCV, user.Id + "_" + "CV" + Path.GetExtension(UploadCV.FileName));
    }

    public async Task OnPostVideoAsync()
    {
        User user = await GetUser() ?? throw new InvalidOperationException("User is not found.");
        await UploadFile(UploadVideo, user.Id + "_" + "Video" + Path.GetExtension(UploadVideo.FileName));
    }

    private async Task UploadFile(IFormFile file, string name)
    {
        string folderPath = Path.Combine(_environment.ContentRootPath, "Storage");
        Directory.CreateDirectory(folderPath);
        string oldFilePath = Path.Combine(folderPath, file.FileName);
        string newFilePath = Path.Combine(folderPath, name);
        using (var fileStream = new FileStream(oldFilePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        System.IO.File.Move(oldFilePath, newFilePath);
    }

    private async Task<User?> GetUser()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        string email = (claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value) ?? throw new InvalidOperationException("User email is not found.");

        return await _authService.FindUserAsync(email);
    }
}
