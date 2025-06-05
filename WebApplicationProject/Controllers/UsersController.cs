using Microsoft.AspNetCore.Mvc;
using WebApplicationProject.Data;
using WebApplicationProject.DTO;

namespace WebApplicationProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    #region [ Private Members ]
    private readonly EmployeeContext _dbContext;

    #endregion

    #region [ Construtor ]

    public UsersController(EmployeeContext dbContext)
    {
        _dbContext = dbContext;
    }

    #endregion

    [HttpGet]
    [ProducesResponseType(200)]
    public ActionResult<IEnumerable<UserDto>> GetUsers()
    {
        List<UserDto> users = _dbContext.Employees.Select(user => new UserDto
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Age = user.Age,
            Email = user.Email
        }).OrderBy(user => user.Id).ToList();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<UserDto> GetUser(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid ID provided.");
        }
        UserDto? userDetails = _dbContext.Employees.Where(user => user.Id == id).Select(user => new UserDto
        {
            Id = user.Id,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Age = user.Age,
            Email = user.Email
        }).FirstOrDefault();
        if (userDetails == null)
        {
            return NotFound();
        }
        return Ok(userDetails);
    }
}
