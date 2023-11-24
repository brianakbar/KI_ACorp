namespace ACorp.Application;

using System.Text;
using ACorp.Data;
using ACorp.Models;
using ACorp.Shared;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

public class DocumentService
{
    readonly IWebHostEnvironment _environment;
    readonly ApplicationDbContext _db;

    public DocumentService(IWebHostEnvironment environment, ApplicationDbContext db)
    {
        _environment = environment;
        _db = db;
    }

    public async Task<(byte[], string)> DownloadAsync(User user, string key, DocumentType type)
    {
        Document? doc;
        if (type == DocumentType.KTP)
        {
            doc = _db.Documents.FirstOrDefault(d => d.UserId == user.Id && d.Cipher == "RC4");
        }
        else if (type == DocumentType.CV)
        {
            doc = _db.Documents.FirstOrDefault(d => d.UserId == user.Id && d.Type == "CV" && d.Cipher == "RC4");
        }
        else
        {
            doc = _db.Documents.FirstOrDefault(d => d.UserId == user.Id && d.Type == "VIDEO" && d.Cipher == "RC4");
        }
        if (doc == null) throw new ApplicationException("Document type: " + type + " from user id: " + user.Id + " not found.");

        var fileName = doc.Name + doc.FileExtension;
        var encryptedRc4FilePath = Path.Combine(_environment.ContentRootPath, "Storage", "rc4", doc.Name);
        using var fileStream = new FileStream(encryptedRc4FilePath, FileMode.Open);
        using MemoryStream decryptedStream = new();
        await fileStream.CopyToAsync(decryptedStream);
        var fileDataEncrypted = decryptedStream.ToArray();
        var myRc4Key = Encoding.UTF8.GetBytes(key);
        var myRc4keyParam = Cryptography.KeyParameterGenerationWithKey(myRc4Key);
        var decryptedFile = Cryptography.Rc4Decrypt(myRc4keyParam, fileDataEncrypted);

        return (decryptedFile, fileName);
    }
}