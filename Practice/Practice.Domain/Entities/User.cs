using System.ComponentModel.DataAnnotations;

namespace Practice.Domain.Entities;

public class User
{
    public int UserId {get;set;}

    [Required]
    public string Email { get; set; } = null!;

    [MaxLength(100)]
    [Required]
    public string Name { get; set; } = null!;
}
