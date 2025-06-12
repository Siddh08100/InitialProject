using projectManagement.API.Models;
using projectManagement.Application.DTO;

namespace projectManagement.Application.Interfaces;

public interface IUserService
{
    Task<object> GetAllUsersAsync(long pageIndex, long pageSize);

    Task<int> UpdateUser(User user);

    Task<int> CreateUser(CreateUser user);

    Task<int> DeleteUser(int id);
    
    Task<UserDto?> FindUserById(int id);
}
