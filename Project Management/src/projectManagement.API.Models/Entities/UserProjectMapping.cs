namespace projectManagement.Domain.Entities;

public class UserProjectMapping
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public string Role { get; set; } = null!;
    public bool IsDeleted { get; set; } = false;
    public User User { get; set; } = null!;
    public Project Project { get; set; } = null!;
}
