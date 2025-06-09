using Microsoft.EntityFrameworkCore;
using projectManagement.Application.Interfaces;
using projectManagement.Domain.Entities;
using projectManagement.Infrastructure.Context;

namespace projectManagement.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    private readonly ProjectManagementContext _context;

    public UserRepository(ProjectManagementContext context)
    {
        _context = context;
    }

    public async Task<(long, List<User>)> GetAllAsync(long? pageIndex, long? pageSize, long? totalCount, long? pageNumber)
    {
        var query = await _context.Users.Where(u => u.IsDeleted == false).ToListAsync();
        long count = query.Count;
        List<User> users = query.Skip((int)((pageIndex - 1) * pageSize)).Take((int)pageSize).ToList();
        return (count, users);
    }

    public async Task<User?> FindByIdAsync(int id)
    {
        return await _context.Users.Where(u => u.Id == id && u.IsDeleted == false).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(User existingUser)
    {
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(User newUser)
    {
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
    }
}
