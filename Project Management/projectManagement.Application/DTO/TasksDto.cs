namespace projectManagement.Application.DTO;

public class TasksDto
{
    public int Id { get; set; }
    public string? ProjectName { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = "Pending";
    public string? UserName { get; set; }
    public DateTime DueDate { get; set; }
    public int UserId { get; set; }
}
