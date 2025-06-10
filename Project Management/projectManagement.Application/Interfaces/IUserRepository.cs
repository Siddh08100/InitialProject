using projectManagement.Domain.Entities;

namespace projectManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<(long,List<User>)> GetAllAsync(long pageIndex, long pageSize);
    Task<User?> FindByIdAsync(int id);
    Task UpdateAsync(User existingUser);
    Task AddAsync(User existingUser);
}

