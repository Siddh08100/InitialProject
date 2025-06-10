using AutoMapper;
using projectManagement.API.Models;
using projectManagement.Application.DTO;
using projectManagement.Application.Interfaces;
using projectManagement.Domain.Entities;

namespace projectManagement.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public ProjectService(IProjectRepository projectRepository, IMapper mapper)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<object> GetProjects(FilterDto filter)
    {
        (FilterDto filters, List<Domain.Entities.Project> projects) = await _projectRepository.GetProjectsAsync(filter);
        var response = new
        {
            paging = new
            {
                pageIndex = filters.PageIndex,
                pageSize = filters.PageSize,
                totalCount = filters.TotalCount,
                pageNumber = filters.PageNumber
            },
            projects = _mapper.Map<List<ProjectDto>>(projects)
        };
        return response;
    }

    public async Task<int> CreateProject(API.Models.Project project)
    {
        Domain.Entities.Project newProject = _mapper.Map<Domain.Entities.Project>(project);
        try
        {
            await _projectRepository.CreateAsync(newProject);
            return 201; // Project Created
        }
        catch (Exception)
        {
            return 400; // Bad Request
        }
    }

    // Soft delete implementation
    public async Task<int> DeleteProject(int id)
    {
        if (id <= 0)
        {
            return 400; // Bad Request
        }
        try
        {
            Domain.Entities.Project? project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return 404; // Not Found
            }
            project.IsDeleted = true; // Soft delete
            await _projectRepository.UpdateAsync();
            return 200; // Project Deleted
        }
        catch (Exception)
        {
            return 0; // Unexpected error
        }
    }

    public async Task<int> UpdateProject(API.Models.Project project)
    {
        if (project == null || project.Id <= 0)
        {
            return 400; // Bad Request
        }
        try
        {
            Domain.Entities.Project? existingProject = await _projectRepository.GetByIdAsync((int)project.Id);
            if (existingProject == null)
            {
                return 404; // Not Found
            }
            existingProject.Name = project.Name;
            existingProject.Description = project.Description;
            existingProject.Status = project.Status;
            existingProject.UpdatedDate = DateTime.Now;
            await _projectRepository.UpdateAsync();
            return 200; // Project Updated
        }
        catch (Exception)
        {
            return 0; // Unexpected error
        }
    }

    // update role of user on particular project [owner,devloper,guest]
    public async Task<int> UpdateUserProjectStatus(UpdateUserProjectStatusRequest updateUserProjectStatusRequest)
    {
        try
        {
            UserProjectMapping? userProject = await _projectRepository.GetUserProjectMappingAsync((int)updateUserProjectStatusRequest.UserId, (int)updateUserProjectStatusRequest.ProjectId);
            if (userProject == null)
            {
                return 404; // User or Project not found
            }
            userProject.Role = updateUserProjectStatusRequest.Status;
            await _projectRepository.UpdateAsync();
            return 200; // User Project Status Updated
        }
        catch
        {
            return 0; // Unexpected error
        }
    }
}
