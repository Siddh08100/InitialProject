using Microsoft.EntityFrameworkCore;
using projectManagement.Application.DTO;
using projectManagement.Domain.Entities;
using projectManagement.Infrastructure.Context;
using projectManagement.Infrastructure.Repository;

namespace projectManagement.Projects;

public class ProjectRepositoryTest
{

    private ProjectManagementContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ProjectManagementContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new ProjectManagementContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Projects.AddRange(new List<Project>
        {
            new() { Id = 1, Name = "A", Description = "react", Status = "Completed",CreatedBy = 2,CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),UpdatedDate = DateTime.Now, IsDeleted = false },
            new() { Id = 2, Name = "B", Description = "Dotnet", Status = "Pending",CreatedBy = 2,CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),UpdatedDate = DateTime.Now, IsDeleted = false },
            new() { Id = 3, Name = "C", Description = "Angular", Status = "In Progress",CreatedBy = 5,CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),UpdatedDate = DateTime.Now, IsDeleted = true },
            new() { Id = 4, Name = "D", Description = "Java", Status = "Pending",CreatedBy = 5,CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),UpdatedDate = DateTime.Now, IsDeleted = false }
        });

        context.UserProjectMappings.AddRange(new List<UserProjectMapping>
        {
            new() { Id = 1, UserId = 1, ProjectId = 1, Role = "Owner",IsDeleted = false },
            new() { Id = 2, UserId = 2, ProjectId = 2, Role = "Developer",IsDeleted = false  },
            new() { Id = 3, UserId = 3, ProjectId = 3, Role = "Guest",IsDeleted = true  },
            new() { Id = 4, UserId = 4, ProjectId = 4, Role = "Developer",IsDeleted = false  }
        });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetProjectsAsync_WithValidInput_ShouldReturnListOfProjects()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new ProjectRepository(context);
        long pageIndex = 1;
        long pageSize = 2;

        FilterDto filter = new()
        {
            PageIndex = (int?)pageIndex,
            PageSize = (int?)pageSize,
        };

        // Act
        var (filters, projects) = await repository.GetProjectsAsync(filter);

        // Assert
        Assert.Equal(3, filters.TotalCount);
        Assert.Equal(2, projects.Count);
        Assert.Equal("A", projects[0].Name);
    }

    [Fact]
    public async Task CreateAsync_WithValidProject_ShouldAddProject()
    {
        var context = GetInMemoryDbContext();
        var repository = new ProjectRepository(context);
        Project project = new()
        {
            Name = "New Project",
            Description = "New Project",
            Status = "Pending",
            CreatedBy = 1,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now,
            IsDeleted = false
        };

        await repository.CreateAsync(project);
        var createdProject = await context.Projects.FirstOrDefaultAsync(p => p.Name == "New Project");

        Assert.NotNull(createdProject);
        Assert.Equal("New Project", createdProject.Name);
    }

    [Fact]
    public async Task CreateAsync_WithNullProject_ShouldThrowException()
    {
        var context = GetInMemoryDbContext();
        var repository = new ProjectRepository(context);
        Project? project = null;

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.CreateAsync(project));

        var projectsCount = await context.Projects.CountAsync();
        Assert.Equal(4, projectsCount); // Ensure no new project was added
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnProject()
    {
        var context = GetInMemoryDbContext();
        var repository = new ProjectRepository(context);
        int projectId = 1;

        var project = await repository.GetByIdAsync(projectId);

        Assert.NotNull(project);
        Assert.Equal("A", project.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var context = GetInMemoryDbContext();
        var repository = new ProjectRepository(context);
        int projectId = 10; // Non-existing project ID

        var project = await repository.GetByIdAsync(projectId);

        Assert.Null(project);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProject_WhenProjectExists()
    {
        var context = GetInMemoryDbContext();
        var repository = new ProjectRepository(context);
        Project? projectToUpdate = await repository.GetByIdAsync(1);
        projectToUpdate.Name = "Updated Project";

        await repository.UpdateAsync();
        await context.SaveChangesAsync();

        var updatedProject = await repository.GetByIdAsync(1);
        Assert.NotNull(updatedProject);
        Assert.Equal("Updated Project", updatedProject.Name);
    }

    [Fact]
    public async Task GetUserProjectMappingAsync_ShouldReturnUserProjectMapping_WhenExists()
    {
        var context = GetInMemoryDbContext();
        var repository = new ProjectRepository(context);
        int userId = 1;
        int projectId = 1;

        var mapping = await repository.GetUserProjectMappingAsync(userId, projectId);

        Assert.NotNull(mapping);
        Assert.Equal(userId, mapping.UserId);
        Assert.Equal(projectId, mapping.ProjectId);
    }

    [Fact]
    public async Task GetUserProjectMappingAsync_WithInvalidId_ShouldReturnNullValue()
    {
        var context = GetInMemoryDbContext();
        var repository = new ProjectRepository(context);
        int userId = 10; // Non-existing user ID
        int projectId = 10; // Non-existing project ID

        var mapping = await repository.GetUserProjectMappingAsync(userId, projectId);

        Assert.Null(mapping);
    }
}
