namespace projectManagement.Application.DTO;

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public int CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
}
