namespace projectManagement.Application.DTO;

public class FilterDto
{
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public int? TotalCount { get; set; }
    public int? PageNumber { get; set; }
    public string? Status {get;set;}  // [Pending, InProgress, Completed]
    public int? UserId { get; set; }
    public int? ProjectId { get; set; }
    public string? Role {get;set;} // [Owner, Developer, Guest]

}
