using projectManagement.API.Models;
using projectManagement.Application.DTO;

namespace projectManagement.Application.Interfaces;

public interface ITaskService
{
    Task<object> GetTasks(FilterDto filter);
    Task<int> CreateTask(CreateTaskRequest createTaskRequest);
    Task<TasksDto> GetTasksAsync(int id);
    Task<int> UpdateTaskAsync(UpdateTaskRequest updateTaskRequest);
}
