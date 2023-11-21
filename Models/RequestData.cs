using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACorp.Models;

public class RequestData
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public int RequesterId { get; set; }

    [ForeignKey("User")]
    public int RequestedId { get; set; }
}