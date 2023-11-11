using ACorp.Data;
using ACorp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;


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

}