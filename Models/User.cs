using System.ComponentModel.DataAnnotations;

namespace KiAcorp.Models;

public class User {
    [Key]
    public int Id {get; set;}

    [Required(ErrorMessage = "Fullname is required")]
    [StringLength(maximumLength:200, MinimumLength =5)]
    public required string Fullname {get;set;}

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email is not valid")]
    public required string Email {get;set;}
    
    public string? AesPassword {get;set;}
    
    public string? Rc4Password {get;set;}
    
    public string? DesPassword {get;set;}
    
    public string? AesPhoneNumber {get;set;}
    
    public string? Rc4PhoneNumber {get;set;}
    
    public string? DesPhoneNumber {get;set;}
    
    public string? AesNik {get;set;}
    
    public string? Rc4Nik {get;set;}
    
    public string? DesNik {get;set;}
   
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime CreatedAt { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTime UpdatedAt { get; set; }

    public List<Document>? Documents {get;set;}
}