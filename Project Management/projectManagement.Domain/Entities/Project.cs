using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projectManagement.Domain.Entities;

public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public string ProjectName { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    public string ProjectStatus { get; set; } = "Pending";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;

}
