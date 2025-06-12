using AutoMapper;
using Moq;
using projectManagement.Application.Interfaces;
using projectManagement.Application.Services;

namespace projectManagement.Projects;

public class ProjectServiceTest
{
    readonly ProjectService _projectService;
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IProjectRepository> _mockUserRepository = new();

    public ProjectServiceTest()
    {
        _projectService = new ProjectService(_mockUserRepository.Object, _mockMapper.Object);
    }


}
