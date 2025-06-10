using Microsoft.EntityFrameworkCore;
using projectManagement.Application.DTO;
using projectManagement.Application.Interfaces;
using projectManagement.Domain.Entities;
using projectManagement.Infrastructure.Context;

namespace projectManagement.Infrastructure.Repository;

public class TaskRepository : ITaskRepository
{
    private readonly ProjectManagementContext _context;
    public TaskRepository(ProjectManagementContext context)
    {
        _context = context;
    }
    public async Task<(FilterDto filters, List<TasksDto> Tasks)> GetTasksAsync(FilterDto filter)
    {
        List<TasksDto> query = await _context.Tasks.Where(t => (t.IsDeleted == false || t.IsDeleted == null) && (filter.ProjectId == null || t.ProjectId == filter.ProjectId) &&
         (filter.UserId == null || filter.UserId == 0 || t.AssignedTo == filter.UserId) && (string.IsNullOrEmpty(filter.Status) || t.Status == filter.Status))
             .OrderBy(t => t.Id).Select(s => new TasksDto
             {
                 Id = s.Id,
                 ProjectName = s.Project.Name,
                 Title = s.Title,
                 Description = s.Description,
                 Status = s.Status,
                 UserName = s.AssignedToUser.FirstName + " " + s.AssignedToUser.LastName,
                 UserId = s.AssignedTo ?? 0,
                 DueDate = s.DueDate ?? DateTime.MaxValue,
             }).ToListAsync();
        filter.TotalCount = query.Count;
        List<TasksDto> newQuery = query.Skip((int)((filter.PageIndex - 1) * filter.PageSize)).Take((int)filter.PageSize).ToList();
        return (filter, newQuery);
    }

    public async Task CreateAsync(Tasks tasks)
    {
        await _context.Tasks.AddAsync(tasks);
        await _context.SaveChangesAsync();
    }

    public async Task<TasksDto?> GetTaskByIdAsync(int id)
    {
        Console.WriteLine($"Fetching task with ID: {id}");
        return await _context.Tasks.Where(t => t.Id == id && (t.IsDeleted == false || t.IsDeleted == null))
            .Select(s => new TasksDto
            {
                Id = s.Id,
                ProjectName = s.Project.Name,
                Title = s.Title,
                Description = s.Description,
                Status = s.Status,
                UserName = s.AssignedToUser.FirstName + " " + s.AssignedToUser.LastName,
                UserId = s.AssignedTo ?? 0,
                DueDate = s.DueDate ?? DateTime.MaxValue,
            }).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(Tasks tasks)
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Tasks?> GetTasksById(int id)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && (t.IsDeleted == false || t.IsDeleted == null));
    }
}
