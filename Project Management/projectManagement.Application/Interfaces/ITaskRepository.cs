using projectManagement.Application.DTO;
using projectManagement.Domain.Entities;

namespace projectManagement.Application.Interfaces;

public interface ITaskRepository
{
    Task<(FilterDto filters,List<TasksDto> Tasks)> GetTasksAsync(FilterDto filter);
    Task CreateAsync(Tasks tasks);
    Task<TasksDto?> GetTaskByIdAsync(int id);
    Task<Tasks?> GetTasksById(int id);
    Task UpdateAsync(Tasks tasks);
}
