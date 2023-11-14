using ACorp.Data;
using ACorp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using ACorp.Shared;
using System.Text;


namespace ACorp.Pages.Dashboard;

[Authorize]
public class DownloadModel : PageModel
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _db;

    public string? KTP { get; set; } = "";
    public string? CV { get; set; } = "";
    public string? Video { get; set; } = "";

    public DownloadModel(IWebHostEnvironment environment, ApplicationDbContext db)
    {
        _environment = environment;
        _db = db;
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
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        var userId = _db.Users.FirstOrDefault(u => u.Email == userEmail)?.Id;
        if (userId == null) return NotFound();

        var ktpDoc = _db.Documents.FirstOrDefault(d => d.UserId == userId && d.Cipher == "RC4");
        if (ktpDoc == null) return NotFound();

        var fileName = ktpDoc.Name + ktpDoc.FileExtension;
        var encryptedRc4FilePath = Path.Combine(_environment.ContentRootPath, "Storage", "rc4", ktpDoc.Name);
        using var fileStream = new FileStream(encryptedRc4FilePath, FileMode.Open);
        using MemoryStream decryptedStream = new();
        await fileStream.CopyToAsync(decryptedStream);
        var fileDataEncrypted = decryptedStream.ToArray();
        var myRc4Key = Encoding.UTF8.GetBytes("ThisIsMyRC4Key");
        var myRc4keyParam = Cryptography.KeyParameterGenerationWithKey(myRc4Key);
        var decryptedFile = Cryptography.Rc4Decrypt(myRc4keyParam, fileDataEncrypted);

        return File(decryptedFile, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> OnPostCv()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        var userId = _db.Users.FirstOrDefault(u => u.Email == userEmail)?.Id;
        if (userId == null) return NotFound();

        var cvDoc = _db.Documents.FirstOrDefault(d => d.UserId == userId && d.Type == "CV" && d.Cipher == "RC4");
        if (cvDoc == null) return NotFound();

        var fileName = cvDoc.Name + cvDoc.FileExtension;
        var encryptedRc4FilePath = Path.Combine(_environment.ContentRootPath, "Storage", "rc4", cvDoc.Name);
        using var fileStream = new FileStream(encryptedRc4FilePath, FileMode.Open);
        using MemoryStream decryptedStream = new();
        await fileStream.CopyToAsync(decryptedStream);
        var fileDataEncrypted = decryptedStream.ToArray();
        var myRc4Key = Encoding.UTF8.GetBytes("ThisIsMyRC4Key");
        var myRc4keyParam = Cryptography.KeyParameterGenerationWithKey(myRc4Key);
        var decryptedFile = Cryptography.Rc4Decrypt(myRc4keyParam, fileDataEncrypted);

        return File(decryptedFile, "application/octet-stream", fileName);
    }

    public async Task<IActionResult> OnPostVideo()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        var userId = _db.Users.FirstOrDefault(u => u.Email == userEmail)?.Id;
        if (userId == null) return NotFound();

        var videoDoc = _db.Documents.FirstOrDefault(d => d.UserId == userId && d.Type == "VIDEO" && d.Cipher == "RC4");
        if (videoDoc == null) return NotFound();

        var fileName = videoDoc.Name + videoDoc.FileExtension;
        var encryptedRc4FilePath = Path.Combine(_environment.ContentRootPath, "Storage", "rc4", videoDoc.Name);
        using var fileStream = new FileStream(encryptedRc4FilePath, FileMode.Open);
        using MemoryStream decryptedStream = new();
        await fileStream.CopyToAsync(decryptedStream);
        var fileDataEncrypted = decryptedStream.ToArray();
        var myRc4Key = Encoding.UTF8.GetBytes("ThisIsMyRC4Key");
        var myRc4keyParam = Cryptography.KeyParameterGenerationWithKey(myRc4Key);
        var decryptedFile = Cryptography.Rc4Decrypt(myRc4keyParam, fileDataEncrypted);

        return File(decryptedFile, "application/octet-stream", fileName);
    }

}