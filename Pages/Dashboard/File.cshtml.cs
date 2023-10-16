using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace ACorp.Pages.Dashboard;

[Authorize]
public class FileModel : PageModel
{
    private IWebHostEnvironment _environment;

    public FileModel(IWebHostEnvironment environment)
    {
        _environment = environment;

    }

    [BindProperty]
    public IFormFile Upload { get; set; }

    public void OnGet()
    {

    }

    public async Task OnPostAsync()
    {
        var file = Path.Combine(_environment.ContentRootPath, "Storage", Upload.FileName);
        using var fileStream = new FileStream(file, FileMode.Create);
        await Upload.CopyToAsync(fileStream);
    }
}
