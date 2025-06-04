namespace WebApplicationProject.DTO;

public class UserDto
{
    public int Id { get; set; }
 
    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public int? Age { get; set; }

    public string Email { get; set; } = null!;
}
