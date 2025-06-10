using AutoMapper;
using projectManagement.API.Models;
using projectManagement.Application.DTO;
using projectManagement.Application.Interfaces;
using projectManagement.Domain.Entities;

namespace projectManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<object> GetTasks(FilterDto filter)
    {
        (FilterDto filters, List<TasksDto> Tasks) = await _taskRepository.GetTasksAsync(filter);
        return new
        {
            paging = new
            {
                pageIndex = filters.PageIndex,
                pageSize = filters.PageSize,
                totalCount = filters.TotalCount,
                pageNumber = filters.PageNumber
            },
            tasks = Tasks
        };
    }

    public async Task<int> CreateTask(CreateTaskRequest createTaskRequest)
    {
        Tasks tasks = _mapper.Map<Tasks>(createTaskRequest);
        try
        {
            await _taskRepository.CreateAsync(tasks);
            return 201;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<TasksDto> GetTasksAsync(int id)
    {
        TasksDto? task = await _taskRepository.GetTaskByIdAsync(id);
        return task;
    }

    public async Task<int> UpdateTaskAsync(UpdateTaskRequest updateTaskRequest)
    {
        if (updateTaskRequest.Id <= 0 || updateTaskRequest == null)
        {
            return 400;
        }
        Tasks? tasks = await _taskRepository.GetTasksById((int)updateTaskRequest.Id);
        if (tasks == null)
        {
            return 404; // Not Found
        }
        tasks.AssignedTo = (int?)updateTaskRequest.AssignedTo;
        tasks.Title = updateTaskRequest.Title;
        tasks.Description = updateTaskRequest.Description;
        tasks.Status = updateTaskRequest.Status;
        tasks.DueDate = updateTaskRequest.DueDate;
        tasks.UpdatedDate = DateTime.UtcNow;
        try
        {
            await _taskRepository.UpdateAsync(tasks);
        }
        catch (Exception)
        {
            return 0; // Unexpected error
        }
        return 200; // OK

    }


}



