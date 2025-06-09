using System;

namespace projectManagement.Domain.Entities;

public class Tasks
{
    public int Id { get; set; }
    public int AssignedTo { get; set; }
    public int ProjectId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public Project Project { get; set; } = null!;
    public User AssignedToUser { get; set; } = null!;
}
