namespace projectManagement.Application.DTO;

public class UsersListsDto
{
    public PagingDto Paging { get; set; } = null!;
    public List<UserDto> Users { get; set; } = null!;
}
