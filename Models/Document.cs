using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KiAcorp.Models;

public class Document {
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "File name is required")]
    public required string Name { get; set; }

    public string? FileExtension {get; set;}

    public string? Type {get; set;}

    [ForeignKey("User")]
    public int UserId {get; set;}

    public required string Cipher {get; set;}

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime UpdatedAt { get; set; }

}