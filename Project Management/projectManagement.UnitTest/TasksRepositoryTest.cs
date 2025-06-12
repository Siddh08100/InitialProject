using Microsoft.EntityFrameworkCore;
using projectManagement.Application.DTO;
using projectManagement.Infrastructure.Context;
using projectManagement.Infrastructure.Repository;

namespace projectManagement.Tasks;

public class TasksRepositoryTest
{
    private ProjectManagementContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ProjectManagementContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new ProjectManagementContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Tasks.AddRange(new List<Domain.Entities.Tasks>
        {
            new() { Id = 1, Title = "Task A", Description = "Description A",ProjectId = 1 ,DueDate = DateTime.Now , Status = "Completed", AssignedTo = 1, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, IsDeleted = false },
            new() { Id = 2, Title = "Task B", Description = "Description B",ProjectId = 2 ,DueDate = DateTime.Now , Status = "Pending", AssignedTo = 2, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, IsDeleted = false },
            new() { Id = 3, Title = "Task C", Description = "Description C",ProjectId = 3 ,DueDate = DateTime.Now , Status = "In Progress", AssignedTo = 3, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, IsDeleted = true },
            new() { Id = 4, Title = "Task D", Description = "Description D",ProjectId = 2 ,DueDate = DateTime.Now , Status = "Pending", AssignedTo = 4, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now, IsDeleted = false }
        });

        context.Users.AddRange(new List<Domain.Entities.User>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "Aleys", Email = "Alice@gmail.com",Password = "Alice@123",Role = "TSE",UserName = "Alice123", IsDeleted = false },
            new() { Id = 2, FirstName = "Bob",  LastName = "Ales", Email = "Bob@gmail.com",Password = "Bob@123",Role = "TSE",UserName = "Bob123", IsDeleted = false },
            new() { Id = 3, FirstName = "Charlie", LastName = "cheys", Email = "Charlie@gmail.com",Password = "Charlie@123",Role = "SSE",UserName = "Charlie123", IsDeleted = true },
            new() { Id = 4, FirstName = "David", LastName = "Heilr", Email = "David@gmail.com",Password = "David@123",Role = "Intern",UserName = "David123", IsDeleted = false }
        });

        context.Projects.AddRange(new List<Domain.Entities.Project>
        {
            new() { Id = 1, Name = "A", Description = "react", Status = "Completed",CreatedBy = 2,CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),UpdatedDate = DateTime.Now, IsDeleted = false },
            new() { Id = 2, Name = "B", Description = "Dotnet", Status = "Pending",CreatedBy = 2,CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),UpdatedDate = DateTime.Now, IsDeleted = false },
            new() { Id = 3, Name = "C", Description = "Angular", Status = "In Progress",CreatedBy = 5,CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),UpdatedDate = DateTime.Now, IsDeleted = true },
            new() { Id = 4, Name = "D", Description = "Java", Status = "Pending",CreatedBy = 5,CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),UpdatedDate = DateTime.Now, IsDeleted = false }
        });

        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetTasksAsync_WithValidInput_ShouldReturnListOfTasks()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new TaskRepository(context);
        int pageIndex = 1;
        int pageSize = 2;

        FilterDto filter = new()
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            ProjectId = 2,
        };

        // Act
        var (filters, tasks) = await repository.GetTasksAsync(filter);

        // Assert
        Assert.Equal(2, filters.TotalCount);
        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public async Task CreateAsync_WithValidInput_ShouldAddTaskToDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new TaskRepository(context);
        var newTask = new Domain.Entities.Tasks
        {
            Title = "New Task",
            Description = "New Task Description",
            ProjectId = 1,
            DueDate = DateTime.Now,
            Status = "Pending",
            AssignedTo = 1,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now,
            IsDeleted = false
        };

        // Act
        await repository.CreateAsync(newTask);
        var taskFromDb = await context.Tasks.FirstOrDefaultAsync(t => t.Title == "New Task");

        // Assert
        Assert.NotNull(taskFromDb);
        Assert.Equal("New Task", taskFromDb.Title);
    }

    [Fact]
    public async Task GetTaskByIdAsync_WithValidId_ShouldReturnTask()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new TaskRepository(context);
        int taskId = 1;

        // Act
        TasksDto? task = await repository.GetTaskByIdAsync(taskId);

        // Assert
        Assert.NotNull(task);
        Assert.Equal("Task A", task.Title);
    }

    [Fact]
    public async Task GetTaskByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new TaskRepository(context);
        int invalidTaskId = 10;

        // Act
        var task = await repository.GetTaskByIdAsync(invalidTaskId);

        // Assert
        Assert.Null(task);
    }

    [Fact]
    public async Task UpdateAsync_WithValidTask_ShouldUpdateTaskInDatabase()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new TaskRepository(context);
        var taskToUpdate = await context.Tasks.FirstOrDefaultAsync(t => t.Id == 1);
        taskToUpdate.Title = "Updated Task A";

        // Act
        await repository.UpdateAsync(taskToUpdate);
        var updatedTask = await context.Tasks.FirstOrDefaultAsync(t => t.Id == 1);

        // Assert
        Assert.NotNull(updatedTask);
        Assert.Equal("Updated Task A", updatedTask.Title);
    }
}
