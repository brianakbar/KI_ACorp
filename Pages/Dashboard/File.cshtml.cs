using System.Security.Claims;
using System.Text;
using KiAcorp.Data;
using KiAcorp.Models;
using KiAcorp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ACorp.Pages.Dashboard;

[Authorize]
public class FileModel : PageModel
{
    private readonly IWebHostEnvironment _environment;
    private readonly ApplicationDbContext _db;

    [BindProperty]
    public IFormFile Upload { get; set; }

    [BindProperty]
    public string FileType { get; set; }
    public FileModel(IWebHostEnvironment environment, ApplicationDbContext db)
    {
        _environment = environment;
        _db = db;
    }

    public void OnGet()
    {
    }

    public async Task OnPostKtpAsync()
    {
        var fileId = Guid.NewGuid().ToString();
        var myKey = Encoding.UTF8.GetBytes("ThisIsAKeyForAES");
        var keyParam = Cryptography.KeyParameterGenerationWithKey(myKey);
        var encryptedFilePath = Path.Combine(_environment.ContentRootPath, "Storage", fileId);

        using MemoryStream encryptionStream = new();
        await Upload.CopyToAsync(encryptionStream);
        var fileData = encryptionStream.ToArray();

        var encryptedFile = Cryptography.AesEcbPaddedEncrypt(keyParam, fileData);

        using var encryptedFileStream = new FileStream(encryptedFilePath, FileMode.Create);
        encryptedFileStream.Write(encryptedFile, 0, encryptedFile.Length);

        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        var userId = _db.Users.FirstOrDefault(u => u.Email == userEmail)?.Id;
        if (userId == null) return;
        Document document = new()
        {
            Name = fileId,
            Cipher = "AES-128",
            FileExtension = Upload.ContentType,
            Type = FileType,
            UserId = (int)userId,
        };
        await _db.Documents.AddAsync(document);
        await _db.SaveChangesAsync();

        // using var fileStream = new FileStream(encryptedFilePath, FileMode.Open);
        // using MemoryStream decryptedStream = new();
        // await fileStream.CopyToAsync(decryptedStream);
        // var fileDataEncrypted = decryptedStream.ToArray();
        // var decryptedFile = Cryptography.AesEcbPaddedDecrypt(keyParam, fileDataEncrypted);
        // var decryptedFilePath = Path.Combine(_environment.ContentRootPath, "Storage", "_decrypted", Upload.FileName);
        // using var decryptedFileStream = new FileStream(decryptedFilePath, FileMode.Create);
        // decryptedFileStream.Write(decryptedFile, 0, decryptedFile.Length);
    }
}
