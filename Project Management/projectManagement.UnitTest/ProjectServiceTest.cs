using AutoMapper;
using Moq;
using projectManagement.API.Models;
using projectManagement.Application.DTO;
using projectManagement.Application.Interfaces;
using projectManagement.Application.Services;
using Task = System.Threading.Tasks.Task;

namespace projectManagement.Projects;

public class ProjectServiceTest
{
    readonly ProjectService _projectService;
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IProjectRepository> _mockProjectRepository = new();

    public ProjectServiceTest()
    {
        _projectService = new ProjectService(_mockProjectRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllProjects_ShouldReturnListOfProjects()   
    {
        // Arrange
        long pageIndex = 1;
        long pageSize = 10;
        List<ProjectDto> projectDtos = new()
        {
            new ProjectDto
            {
                Id = 3,
                Name = "Project",
                Description = "Task",
                Status = "In Progress",
                CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
                CreatedBy = 2,
                UpdatedDate = DateTime.MinValue
            },
        };

        List<Domain.Entities.Project> projects = new()
        {
            new Domain.Entities.Project
            {
                Id = 3,
                Name = "Project",
                Description = "Task",
                Status = "In Progress",
                CreatedBy = 2,
                CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
                UpdatedDate = DateTime.MinValue,
                IsDeleted = false
            },
        };

        FilterDto filter = new()
        {
            PageIndex = (int?)pageIndex,
            PageSize = (int?)pageSize,
            TotalCount = projects.Count
        };

        var expectedprojects = new
        {
            Paging = new
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = projects.Count,
                PageNumber = (int)Math.Ceiling((decimal)((double)projects.Count / pageSize))
            },
            List = projectDtos
        };

        // Act
        _mockProjectRepository.Setup(us => us.GetProjectsAsync(filter)).ReturnsAsync((filter, projects));
        _mockMapper.Setup(m => m.Map<List<ProjectDto>>(projects)).Returns(projectDtos);
        GetAllDto<ProjectDto>? result = await _projectService.GetProjects(filter) as GetAllDto<ProjectDto>;

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
            Assert.Equal(expectedprojects.List[i].Name, result.List[i].Name);
            Assert.Equal(expectedprojects.List[i].Description, result.List[i].Description);
            Assert.Equal(expectedprojects.List[i].Status, result.List[i].Status);
            Assert.Equal(expectedprojects.List[i].CreatedBy, result.List[i].CreatedBy);
            Assert.Equal(expectedprojects.List[i].CreatedDate, result.List[i].CreatedDate);
        }
    }

    [Fact]
    public async Task CreateProject_WithValidInput_ShouldCreateProject()
    {
        // Arrange
        API.Models.CreateProject NewProjectModel = new()
        {
            Name = "Project",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
        };

        Domain.Entities.Project projects = new()
        {
            Name = "Project",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
        };

        //Act
        _mockMapper.Setup(m => m.Map<Domain.Entities.Project>(NewProjectModel)).Returns(projects);
        _mockProjectRepository.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.Project>())).Returns(Task.CompletedTask);

        int result = await _projectService.CreateProject(NewProjectModel);


        // Assert
        Assert.Equal(201, result);
        _mockProjectRepository.Verify(r => r.CreateAsync(It.Is<Domain.Entities.Project>(
            u => u.Name == NewProjectModel.Name &&
                 u.Description == NewProjectModel.Description &&
                 u.Status == NewProjectModel.Status &&
                 u.CreatedBy == NewProjectModel.CreatedBy &&
                 u.CreatedDate == NewProjectModel.CreatedDate)), Times.Once);
    }

    [Fact]
    public async Task CreateProject_WithCreateAsyncThrowsException_ShouldReturnBadRequest()
    {

        API.Models.CreateProject NewProjectModel = new()
        {
            Name = "Project",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
        };

        Domain.Entities.Project projects = new()
        {
            Name = "Project",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
        };

        //Act
        _mockMapper.Setup(m => m.Map<Domain.Entities.Project>(NewProjectModel)).Returns(projects);
        _mockProjectRepository.Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.Project>())).ThrowsAsync(new Exception("Database update failed"));

        int result = await _projectService.CreateProject(NewProjectModel);


        // Assert
        Assert.Equal(400, result);
        _mockProjectRepository.Verify(r => r.CreateAsync(It.IsAny<Domain.Entities.Project>()), Times.Once);
    }

    [Fact]
    public async Task CreateProject_WithInValidMapping_ShouldThrowError()
    {
        //Arrange
        API.Models.CreateProject NewProjectModel = new()
        {
            Name = "Project",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
        };

        _mockMapper.Setup(m => m.Map<Domain.Entities.Project>(It.IsAny<API.Models.CreateProject>()))
           .Throws(new AutoMapperMappingException("Mapping failed"));

        // Act
        var result = await _projectService.CreateProject(NewProjectModel);

        // Assert
        Assert.Equal(400, result); // Bad request due to mapping error
        _mockProjectRepository.Verify(r => r.CreateAsync(It.IsAny<Domain.Entities.Project>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProject_WhenUserExists_ReturnsSuccess()
    {
        // Arrange
        int projectId = 3;
        Domain.Entities.Project project = new()
        {
            Id = 3,
            Name = "Project",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
            IsDeleted = false
        };
        _mockProjectRepository.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _mockProjectRepository.Setup(r => r.UpdateAsync()).Returns(Task.CompletedTask);

        // Act
        int result = await _projectService.DeleteProject(projectId);

        // Assert
        Assert.Equal(200, result);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteProject_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        int projectId = 5;

        // Act
        _mockProjectRepository.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync((Domain.Entities.Project?)null);
        int result = await _projectService.DeleteProject(projectId);

        // Assert
        Assert.Equal(404, result);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteProject_WhenUserIdIsZero_ReturnsBadRequest()
    {
        // Arrange
        int projectId = 0;

        // Act
        int result = await _projectService.DeleteProject(projectId);

        // Assert
        Assert.Equal(400, result);
        _mockProjectRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteProject_WhenUpdateAsyncThrowsException_ReturnsBadRequest()
    {
        int projectId = 2;
        Domain.Entities.Project project = new()
        {
            Id = 3,
            Name = "Project",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
            IsDeleted = false
        };


        _mockProjectRepository.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
        _mockProjectRepository.Setup(r => r.UpdateAsync()).ThrowsAsync(new Exception("Database update failed"));

        // Act
        int result = await _projectService.DeleteProject(projectId);

        // Assert
        Assert.Equal(0, result);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Once);
    }


    [Fact]
    public async Task UpdateProject_WithValidInput_ShouldUpdateProject()
    {
        // Arrange
        Domain.Entities.Project project = new()
        {
            Id = 3,
            Name = "Project",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
            IsDeleted = false
        };

        API.Models.Project modelsProject = new()
        {
            Id = 3,
            Name = "Project1",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
        };

        // Act
        _mockProjectRepository.Setup(r => r.GetByIdAsync((int)modelsProject.Id)).ReturnsAsync(project);
        _mockProjectRepository.Setup(r => r.UpdateAsync()).Returns(Task.CompletedTask);

        var result = await _projectService.UpdateProject(modelsProject);

        // Assert
        Assert.Equal(200, result);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProject_WithInValidProjectId_ShouldReturnNotFound()
    {
        API.Models.Project modelsProject = new()
        {
            Id = 1,
            Name = "Project1",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
        };

        // Act
        _mockProjectRepository.Setup(r => r.GetByIdAsync((int)modelsProject.Id)).ReturnsAsync((Domain.Entities.Project?)null);
        var result = await _projectService.UpdateProject(modelsProject);

        // Assert
        Assert.Equal(404, result);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateProject_WithProjectIdIsZero_ShouldReturnBadRequest()
    {
        API.Models.Project modelsProject = new()
        {
            Id = 0,
            Name = "Project1",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
        };
        // Act
        var result = await _projectService.UpdateProject(modelsProject);

        // Assert
        Assert.Equal(400, result);
        _mockProjectRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateProject_WhenUpdateAsyncThrowsException_ShouldReturnBadRequest()
    {
        // Arrange
        Domain.Entities.Project project = new()
        {
            Id = 3,
            Name = "Project",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
            IsDeleted = false
        };

        Project inputProject = new()
        {
            Id = 3,
            Name = "Project1",
            Description = "Task",
            Status = "In Progress",
            CreatedBy = 2,
            CreatedDate = DateTime.Parse("2025-06-10T07:35:29.004302Z"),
            UpdatedDate = DateTime.MinValue,
        };

        // Mock FindByIdAsync to return a valid user
        _mockProjectRepository.Setup(r => r.GetByIdAsync((int)inputProject.Id)).ReturnsAsync(project);

        // Mock UpdateAsync to throw an exception
        _mockProjectRepository.Setup(r => r.UpdateAsync()).ThrowsAsync(new Exception("Database update failed"));

        // Act
        int result = await _projectService.UpdateProject(inputProject);

        // Assert
        Assert.Equal(0, result); // Exception result in Bad Request
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Once);

    }

    [Fact]
    public async Task UpdateUserProjectStatus_WithValidInput_ShouldUpdateStatus()
    {
        // Arrange
        UpdateUserProjectStatusRequest updateUserProjectStatusRequest = new()
        {
            ProjectId = 3,
            UserId = 2,
            Status = "Developer"
        };

        Domain.Entities.UserProjectMapping userProjectMapping = new()
        {
            Id = 3,
            UserId = 2,
            ProjectId = 3,
            Role = "Guest",
            IsDeleted = false
        };

        // Act
        _mockProjectRepository.Setup(r => r.GetUserProjectMappingAsync((int)updateUserProjectStatusRequest.UserId, (int)updateUserProjectStatusRequest.ProjectId)).ReturnsAsync(userProjectMapping);
        _mockProjectRepository.Setup(r => r.UpdateAsync()).Returns(Task.CompletedTask);

        int result = await _projectService.UpdateUserProjectStatus(updateUserProjectStatusRequest);

        // Assert
        Assert.Equal(200, result);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUserProjectStatus_WithInvalidUserIdOrProjectId_ShouldReturnBadRequest()
    {
        // Arrange
        UpdateUserProjectStatusRequest updateUserProjectStatusRequest = new()
        {
            ProjectId = 0,
            UserId = 2,
            Status = "Developer"
        };

        // Act
        int result = await _projectService.UpdateUserProjectStatus(updateUserProjectStatusRequest);

        // Assert
        Assert.Equal(400, result);
        _mockProjectRepository.Verify(r => r.GetUserProjectMappingAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateUserProjectStatus_WhenUserProjectMappingNotFound_ShouldReturnNotFound()
    {
        // Arrange
        UpdateUserProjectStatusRequest updateUserProjectStatusRequest = new()
        {
            ProjectId = 3,
            UserId = 2,
            Status = "Developer"
        };

        // Act
        _mockProjectRepository.Setup(r => r.GetUserProjectMappingAsync((int)updateUserProjectStatusRequest.UserId, (int)updateUserProjectStatusRequest.ProjectId)).ReturnsAsync((Domain.Entities.UserProjectMapping?)null);
        int result = await _projectService.UpdateUserProjectStatus(updateUserProjectStatusRequest);

        // Assert
        Assert.Equal(404, result);
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateUserProjectStatus_WhenUpdateAsyncThrowsException_ShouldReturnBadRequest()
    {
        // Arrange
        UpdateUserProjectStatusRequest updateUserProjectStatusRequest = new()
        {
            ProjectId = 3,
            UserId = 2,
            Status = "Developer"
        };

        Domain.Entities.UserProjectMapping userProjectMapping = new()
        {
            Id = 3,
            UserId = 2,
            ProjectId = 3,
            Role = "Guest",
            IsDeleted = false
        };

        _mockProjectRepository.Setup(r => r.GetUserProjectMappingAsync((int)updateUserProjectStatusRequest.UserId, (int)updateUserProjectStatusRequest.ProjectId)).ReturnsAsync(userProjectMapping);
        _mockProjectRepository.Setup(r => r.UpdateAsync()).ThrowsAsync(new Exception("Database update failed")); // Throws Error

        // Act
        int result = await _projectService.UpdateUserProjectStatus(updateUserProjectStatusRequest);

        // Assert
        Assert.Equal(0, result); // Exception result in Bad Request
        _mockProjectRepository.Verify(r => r.UpdateAsync(), Times.Once);
    }
}
