namespace projectManagement.Application.DTO;


public class GetAllDto<T>
{
    public PagingDto Paging { get; set; } = null!;
    public List<T> List { get; set; } = null!;
}

