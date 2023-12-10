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
    readonly RequestDataService _requestDataService;

    public DocumentService(IWebHostEnvironment environment, ApplicationDbContext db)
    {
        _environment = environment;
        _db = db;
        _requestDataService = new RequestDataService(_db);
    }

    public async Task<(byte[], string)> DownloadAsync(User user, string key, DocumentType type)
    {
        Document? doc;
        if (type == DocumentType.KTP)
        {
            doc = _db.Documents.FirstOrDefault(d => d.UserId == user.Id && d.Type == "ID_CARD" && d.Cipher == "RC4");
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
        var fileDataSigned = decryptedStream.ToArray();

        var fileDataEncrypted = ExtractDataFromDigitalSignature(user, fileDataSigned);

        var myRc4Key = Encoding.UTF8.GetBytes(key);
        var myRc4keyParam = Cryptography.KeyParameterGenerationWithKey(myRc4Key);
        var decryptedFile = Cryptography.Rc4Decrypt(myRc4keyParam, fileDataEncrypted);

        return (decryptedFile, fileName);
    }

    public async Task<byte[]> AttachDigitalSignatureAsync(User sender, byte[] data)
    {
        byte[] digitalSignature = await _requestDataService.GenerateDigitalSignatureAsync(sender, data);

        byte[] combinedData = new byte[data.Length + digitalSignature.Length + 8]; // 8 bytes for storing array lengths

        BitConverter.GetBytes(data.Length).CopyTo(combinedData, 0);
        BitConverter.GetBytes(digitalSignature.Length).CopyTo(combinedData, 4);
        data.CopyTo(combinedData, 8);
        digitalSignature.CopyTo(combinedData, 8 + data.Length);

        return combinedData;
    }

    public byte[] ExtractDataFromDigitalSignature(User sender, byte[] combinedData)
    {
        int dataLength = BitConverter.ToInt32(combinedData, 0);
        int digitalSignatureLength = BitConverter.ToInt32(combinedData, 4);

        byte[] data = new byte[dataLength];
        byte[] digitalSignature = new byte[digitalSignatureLength];

        Array.Copy(combinedData, 8, data, 0, dataLength);
        Array.Copy(combinedData, 8 + dataLength, digitalSignature, 0, digitalSignatureLength);

        if (!_requestDataService.VerifyDigitalSignature(sender, data, digitalSignature)) throw new ApplicationException("Data has been tempered!");

        return data;
    }
}