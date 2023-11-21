using System.Security.Claims;
using System.Text;
using ACorp.Data;
using ACorp.Models;
using ACorp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        var userId = _db.Users.FirstOrDefault(u => u.Email == userEmail)?.Id;
        var idCards = _db.Documents.Where(d => d.UserId == userId).Where(d => d.Type == "ID_CARD");
        var Cvs = _db.Documents.Where(d => d.UserId == userId).Where(d => d.Type == "CV");
        var videos = _db.Documents.Where(d => d.UserId == userId).Where(d => d.Type == "VIDEO");
    }

    public async Task OnPostAsync()
    {
        ClaimsIdentity? claimsIdentity = User.Identity as ClaimsIdentity;
        var userEmail = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value;
        var userId = _db.Users.FirstOrDefault(u => u.Email == userEmail)?.Id;
        if (userId == null) return;

        var fileId = Guid.NewGuid().ToString();
        var fileExtension = Path.GetExtension(Upload.FileName);

        // remove previous file
        var prevDoc = _db.Documents.FirstOrDefault(d => d.UserId == userId && d.FileExtension == fileExtension);
        if (prevDoc != null)
        {
            String[] subPaths = new String[] { "aes", "des", "rc4" };
            foreach (String subPath in subPaths)
            {
                var prevFilePath = Path.Combine(_environment.ContentRootPath, "Storage", subPath, prevDoc.Name);
                if (System.IO.File.Exists(prevFilePath))
                {
                    System.IO.File.Delete(prevFilePath);
                }
            }
            _db.RemoveRange(_db.Documents.Where(d => d.Name == prevDoc.Name).ToList());

        }

        var myAesKey = Encoding.UTF8.GetBytes("ThisIsAKeyForAES");
        var myAesIv = Encoding.UTF8.GetBytes("ThisIVAKeyForAES");
        var myAeskeyParam = Cryptography.KeyParameterGenerationWithKeyAndIV(myAesKey, myAesIv);

        var myDesKey = Encoding.UTF8.GetBytes("KyForDES");
        var myDesIv = Encoding.UTF8.GetBytes("IvForDES");
        var myDeskeyParam = Cryptography.KeyParameterGenerationWithKeyAndIV(myDesKey, myDesIv);

        var myRc4Key = Encoding.UTF8.GetBytes("ThisIsMyRC4Key");
        var myRc4keyParam = Cryptography.KeyParameterGenerationWithKey(myRc4Key);

        using MemoryStream encryptionStream = new();
        await Upload.CopyToAsync(encryptionStream);
        var fileData = encryptionStream.ToArray();

        // encrypt with rc4
        DateTime startRc4 = DateTime.Now;
        var encryptedRc4File = Cryptography.Rc4Encrypt(myRc4keyParam, fileData);
        DateTime endRc4 = DateTime.Now;
        var encryptedRc4FilePath = Path.Combine(_environment.ContentRootPath, "Storage", "rc4", fileId);
        using var encryptedRc4FileStream = new FileStream(encryptedRc4FilePath, FileMode.Create);
        encryptedRc4FileStream.Write(encryptedRc4File, 0, encryptedRc4File.Length);
        Document document3 = new()
        {
            Name = fileId,
            Cipher = "RC4",
            FileExtension = fileExtension,
            Type = FileType,
            UserId = (int)userId,
            EncryptDuration = endRc4 - startRc4
        };
        await _db.Documents.AddAsync(document3);

        // encrypt with des cbc
        DateTime startDes = DateTime.Now;
        var encryptedDesFile = Cryptography.DesCbcPaddedEncrypt(myDeskeyParam, fileData);
        DateTime endDes = DateTime.Now;
        var encryptedDesFilePath = Path.Combine(_environment.ContentRootPath, "Storage", "des", fileId);
        using var encryptedDesFileStream = new FileStream(encryptedDesFilePath, FileMode.Create);
        encryptedDesFileStream.Write(encryptedDesFile, 0, encryptedDesFile.Length);
        Document document2 = new()
        {
            Name = fileId,
            Cipher = "DES-64-CBC",
            FileExtension = fileExtension,
            Type = FileType,
            UserId = (int)userId,
            EncryptDuration = endDes - startDes
        };
        await _db.Documents.AddAsync(document2);

        // encrypt with aes 128 cbc
        DateTime startAes = DateTime.Now;
        var encryptedAesFile = Cryptography.AesCbcPaddedEncrypt(myAeskeyParam, fileData);
        DateTime endAes = DateTime.Now;
        var encryptedAesFilePath = Path.Combine(_environment.ContentRootPath, "Storage", "aes", fileId);
        using var encryptedFileStream = new FileStream(encryptedAesFilePath, FileMode.Create);
        encryptedFileStream.Write(encryptedAesFile, 0, encryptedAesFile.Length);
        Document document = new()
        {
            Name = fileId,
            Cipher = "AES-128-CBC",
            FileExtension = fileExtension,
            Type = FileType,
            UserId = (int)userId,
            EncryptDuration = endAes - startAes
        };
        await _db.Documents.AddAsync(document);

        await _db.SaveChangesAsync();
    }
}
