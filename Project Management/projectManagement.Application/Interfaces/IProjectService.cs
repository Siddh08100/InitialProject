using projectManagement.API.Models;
using projectManagement.Application.DTO;

namespace projectManagement.Application.Interfaces;

public interface IProjectService
{
    Task<object> GetProjects(FilterDto filter);
    Task<int> CreateProject(CreateProject project);
    Task<int> DeleteProject(int id);
    Task<int> UpdateProject(Project project);
    Task<int> UpdateUserProjectStatus(UpdateUserProjectStatusRequest updateUserProjectStatusRequest);
}
