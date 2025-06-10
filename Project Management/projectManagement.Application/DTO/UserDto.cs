namespace projectManagement.Application.DTO;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!; 
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}
