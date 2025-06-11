namespace projectManagement.Application.DTO;

public class PagingDto
{
    public long PageIndex { get; set; }
    public long PageSize { get; set; }
    public long TotalCount { get; set; }
    public long PageNumber { get; set; }
}
