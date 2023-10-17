namespace ACorp.Application;

using KiAcorp.Data;
using KiAcorp.Models;
using Microsoft.EntityFrameworkCore;

public class DocumentService
{
    const string DocumentFolderPath = "Storage";

    readonly ApplicationDbContext _db;
    readonly IWebHostEnvironment _environment;

    public DocumentService(ApplicationDbContext db, IWebHostEnvironment environment)
    {
        _db = db;
        _environment = environment;
    }

    public string GetDocumentPath(Document document)
    {
        return Path.Combine(_environment.ContentRootPath, DocumentFolderPath, document.Name);
    }

    public async Task<Document?> GetDocumentByTypeAsync(User user, DocumentType type)
    {
        Document? foundDocument = null;

        await _db.Documents.Where(document => document.UserId == user.Id).ForEachAsync(document =>
        {
            if (document.Type == type.ToString()) foundDocument = document;
        });

        return foundDocument;
    }

    public async Task UploadDocumentAsync(User user, IFormFile file, string name, DocumentType type)
    {
        string folderPath = Path.Combine(_environment.ContentRootPath, DocumentFolderPath);
        Directory.CreateDirectory(folderPath);
        string oldFilePath = Path.Combine(folderPath, file.FileName);
        string newFilePath = Path.Combine(folderPath, name);
        using (var fileStream = new FileStream(oldFilePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        Document? oldDocument = await GetDocumentByTypeAsync(user, type);
        Document newDocument = new()
        {
            Name = name,
            FileExtension = Path.GetExtension(newFilePath),
            Type = type.ToString(),
            Cipher = "None"
        };
        if (oldDocument == null)
        {
            await AddDocumentAsync(user, newDocument);
        }
        else
        {
            File.Delete(GetDocumentPath(oldDocument));
            await ReplaceDocumentAsync(user, oldDocument, newDocument);
        }
        File.Move(oldFilePath, newFilePath);
    }

    public async Task AddDocumentAsync(User user, Document document)
    {
        user.Documents ??= new();
        user.Documents.Add(document);
        await _db.SaveChangesAsync();
    }

    public async Task ReplaceDocumentAsync(User user, Document oldDocument, Document newDocument)
    {
        await DeleteDocumentAsync(user, oldDocument);
        await AddDocumentAsync(user, newDocument);
    }

    public async Task DeleteDocumentAsync(User user, Document documentToDelete)
    {
        user.Documents?.Remove(documentToDelete);
        await _db.SaveChangesAsync();
    }
}