
using projectManagement.Application.DTO;
using projectManagement.Domain.Entities;

namespace projectManagement.Application.Interfaces;

public interface IProjectRepository
{
    Task<(FilterDto filters , List<Project> projects)> GetProjectsAsync(FilterDto filter);
    Task CreateAsync(Project project);
    Task<Project?> GetByIdAsync(int id);
    Task UpdateAsync();
    Task<UserProjectMapping?> GetUserProjectMappingAsync(int userId, int projectId);

}
