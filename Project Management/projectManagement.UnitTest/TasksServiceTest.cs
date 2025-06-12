using AutoMapper;
using Moq;
using projectManagement.API.Models;
using projectManagement.Application.DTO;
using projectManagement.Application.Interfaces;
using projectManagement.Application.Services;
using projectManagement.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace projectManagement.Tasks;

public class TasksServiceTest
{
    private readonly TaskService _tasksService;
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<ITaskRepository> _tasksRepositoryMock;

    public TasksServiceTest()
    {
        _tasksRepositoryMock = new Mock<ITaskRepository>();
        _tasksService = new TaskService(_tasksRepositoryMock.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnListOfTasks()
    {
        // Arrange
        long pageIndex = 1;
        long pageSize = 10;
        List<TasksDto> taskDtos =
        [
            new TasksDto
            {
                Id = 3,
                ProjectName = "Project",
                Description = "Task",
                Status = "In Progress",
                UserName = "John Doe",
                DueDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
                UserId = 2,
                Title = "Task Title"
            },
         ];

        FilterDto filter = new()
        {
            PageIndex = (int?)pageIndex,
            PageSize = (int?)pageSize,
            TotalCount = taskDtos.Count,
        };

        var expectedprojects = new
        {
            Paging = new
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = taskDtos.Count,
                PageNumber = (int)Math.Ceiling((decimal)((double)taskDtos.Count / pageSize))
            },
            List = taskDtos
        };

        // Act
        _tasksRepositoryMock.Setup(us => us.GetTasksAsync(filter)).ReturnsAsync((filter, taskDtos));
        GetAllDto<TasksDto>? result = await _tasksService.GetTasks(filter) as GetAllDto<TasksDto>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedprojects.Paging.PageIndex, result.Paging.PageIndex);
        Assert.Equal(expectedprojects.Paging.PageSize, result.Paging.PageSize);
        Assert.Equal(expectedprojects.Paging.TotalCount, result.Paging.TotalCount);
        Assert.Equal(expectedprojects.Paging.PageNumber, result.Paging.PageNumber);
        Assert.Equal(expectedprojects.List.Count, result.List.Count);
        for (int i = 0; i < expectedprojects.List.Count; i++)
        {
            Assert.Equal(expectedprojects.List[i].Id, result.List[i].Id);
            Assert.Equal(expectedprojects.List[i].Title, result.List[i].Title);
            Assert.Equal(expectedprojects.List[i].ProjectName, result.List[i].ProjectName);
            Assert.Equal(expectedprojects.List[i].Description, result.List[i].Description);
            Assert.Equal(expectedprojects.List[i].Status, result.List[i].Status);
            Assert.Equal(expectedprojects.List[i].UserId, result.List[i].UserId);
            Assert.Equal(expectedprojects.List[i].DueDate, result.List[i].DueDate);
            Assert.Equal(expectedprojects.List[i].UserName, result.List[i].UserName);
        }
    }

    [Fact]
    public async Task CreateTask_WithValidInput_ShouldReturnSuccess()
    {
        // Arrange
        CreateTaskRequest createTaskRequest = new()
        {
            Title = "New Task",
            Description = "Task Description",
            Status = "Open",
            DueDate = DateTime.UtcNow.AddDays(7),
            AssignedTo = 1,
            ProjectId = 1
        };

        Domain.Entities.Tasks tasks = new()
        {
            Title = "New Task",
            Description = "Task Description",
            Status = "Open",
            DueDate = DateTime.UtcNow.AddDays(7),
            AssignedTo = 1,
            ProjectId = 1,
            CreatedDate = DateTime.Now,
        };

        _mockMapper.Setup(m => m.Map<Domain.Entities.Tasks>(createTaskRequest)).Returns(tasks);
        _tasksRepositoryMock.Setup(repo => repo.CreateAsync(tasks)).Returns(Task.CompletedTask);

        // Act
        int result = await _tasksService.CreateTask(createTaskRequest);

        // Assert
        Assert.Equal(201, result);
    }

    [Fact]
    public async Task CreateTask_WithCreateAsyncThrowsException_ShouldReturnBadRequest()
    {
        // Arrange
        CreateTaskRequest createTaskRequest = new()
        {
            Title = "",
            Description = "Task Description",
            Status = "Open",
            DueDate = DateTime.UtcNow.AddDays(7),
            AssignedTo = 1,
            ProjectId = 1
        };

        Domain.Entities.Tasks tasks = new()
        {
            Title = "",
            Description = "Task Description",
            Status = "Open",
            DueDate = DateTime.UtcNow.AddDays(7),
            AssignedTo = 1,
            ProjectId = 1,
            CreatedDate = DateTime.Now,
        };

        _mockMapper.Setup(m => m.Map<Domain.Entities.Tasks>(createTaskRequest)).Returns(tasks);
        _tasksRepositoryMock.Setup(r => r.CreateAsync(tasks)).ThrowsAsync(new Exception("Database update failed"));
        // Act
        int result = await _tasksService.CreateTask(createTaskRequest);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CreateTask_WithInValidMapping_ShouldThrowError()
    {
        //Arrange
        CreateTaskRequest createTaskRequest = new()
        {
            Title = "New Task",
            Description = "Task Description",
            Status = "Open",
            DueDate = DateTime.UtcNow.AddDays(7),
            AssignedTo = 1,
            ProjectId = 1
        };

        _mockMapper.Setup(m => m.Map<Domain.Entities.Tasks>(It.IsAny<CreateTaskRequest>()))
           .Throws(new AutoMapperMappingException("Mapping failed"));

        // Act
        var result = await _tasksService.CreateTask(createTaskRequest);

        // Assert
        Assert.Equal(0, result); // Bad request due to mapping error
        _tasksRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Domain.Entities.Tasks>()), Times.Never);
    }

    [Fact]
    public async Task GetTasksAsync_withValidId_ShouldReturnTask()
    {
        // Arrange
        int taskId = 1;
        TasksDto expectedTask = new()
        {
            Id = taskId,
            ProjectName = "Project",
            Title = "Task Title",
            Description = "Task Description",
            Status = "Open",
            UserName = "John Doe",
            UserId = 2,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        _tasksRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync(expectedTask);

        // Act
        TasksDto result = await _tasksService.GetTasksAsync(taskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTask.Id, result.Id);
        Assert.Equal(expectedTask.ProjectName, result.ProjectName);
        Assert.Equal(expectedTask.Title, result.Title);
        Assert.Equal(expectedTask.Description, result.Description);
        Assert.Equal(expectedTask.Status, result.Status);
        Assert.Equal(expectedTask.UserName, result.UserName);
        Assert.Equal(expectedTask.UserId, result.UserId);
        Assert.Equal(expectedTask.DueDate, result.DueDate);
    }

    [Fact]
    public async Task GetTasksAsync_withInvalidId_ShouldReturnNull()
    {
        // Arrange
        int taskId = 15; // Assuming this ID does not exist
        _tasksRepositoryMock.Setup(repo => repo.GetTaskByIdAsync(taskId)).ReturnsAsync((TasksDto?)null);

        // Act
        TasksDto? result = await _tasksService.GetTasksAsync(taskId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithValidInput_ShouldUpdateTask()
    {
        // Arrange
        UpdateTaskRequest updateTaskRequest = new()
        {
            Id = 1,
            Title = "Updated Task",
            Description = "Updated Description",
            Status = "In Progress",
            DueDate = DateTime.UtcNow.AddDays(3),
            AssignedTo = 2
        };

        Domain.Entities.Tasks existingTask = new()
        {
            Id = 1,
            Title = "Old Task",
            Description = "Old Description",
            Status = "Open",
            DueDate = DateTime.UtcNow.AddDays(5),
            AssignedTo = 1,
            UpdatedDate = DateTime.UtcNow
        };

        _tasksRepositoryMock.Setup(repo => repo.GetTasksById((int)updateTaskRequest.Id)).ReturnsAsync(existingTask);
        _tasksRepositoryMock.Setup(repo => repo.UpdateAsync(existingTask)).Returns(Task.CompletedTask);

        // Act
        int result = await _tasksService.UpdateTaskAsync(updateTaskRequest);

        // Assert
        Assert.Equal(200, result);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithTaskIdIsZero_ShouldReturnBadRequest()
    {
        // Arrange
        UpdateTaskRequest updateTaskRequest = new()
        {
            Id = 0,
            Title = "Updated Task",
            Description = "Updated Description",
            Status = "In Progress",
            DueDate = DateTime.UtcNow.AddDays(3),
            AssignedTo = 2
        };

        // Act
        int result = await _tasksService.UpdateTaskAsync(updateTaskRequest);

        // Assert
        Assert.Equal(400, result); // Bad Request due to invalid ID
    }

    [Fact]
    public async Task UpdateTaskAsync_WithInValidTaskId_ShouldReturnNotFound()
    {
        UpdateTaskRequest updateTaskRequest = new()
        {
            Id = 8,
            Title = "Updated Task",
            Description = "Updated Description",
            Status = "In Progress",
            DueDate = DateTime.UtcNow.AddDays(3),
            AssignedTo = 2
        };

        // Act
        _tasksRepositoryMock.Setup(r => r.GetTasksById((int)updateTaskRequest.Id)).ReturnsAsync((Domain.Entities.Tasks?)null);
        var result = await _tasksService.UpdateTaskAsync(updateTaskRequest);

        // Assert
        Assert.Equal(404, result);
        _tasksRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Domain.Entities.Tasks>(
            u => u.Title == updateTaskRequest.Title &&
                 u.Description == updateTaskRequest.Description &&
                 u.Id == updateTaskRequest.Id &&
                 u.ProjectId == updateTaskRequest.ProjectId &&
                 u.Status == updateTaskRequest.Status &&
                 u.AssignedTo == updateTaskRequest.AssignedTo)), Times.Never);
    }

    [Fact]
    public async Task UpdateTaskAsync_WhenUpdateAsyncThrowsException_ShouldReturnBadRequest()
    {
        UpdateTaskRequest updateTaskRequest = new()
        {
            Id = 1,
            Title = "Updated Task",
            Description = "Updated Description",
            Status = "In Progress",
            DueDate = DateTime.UtcNow.AddDays(3),
            AssignedTo = 2
        };

        Domain.Entities.Tasks existingTask = new()
        {
            Id = 1,
            Title = "Old Task",
            Description = "Old Description",
            Status = "Open",
            DueDate = DateTime.UtcNow.AddDays(5),
            AssignedTo = 1,
            UpdatedDate = DateTime.UtcNow
        };

        _tasksRepositoryMock.Setup(r => r.GetTasksById((int)updateTaskRequest.Id)).ReturnsAsync(existingTask);

        // Mock UpdateAsync to throw an exception
        _tasksRepositoryMock.Setup(r => r.UpdateAsync(existingTask)).ThrowsAsync(new Exception("Database update failed"));

        // Act
        int result = await _tasksService.UpdateTaskAsync(updateTaskRequest);

        // Assert
        Assert.Equal(0, result); // Exception result in Bad Request
        _tasksRepositoryMock.Verify(r => r.UpdateAsync(existingTask), Times.Once);
    }
}
