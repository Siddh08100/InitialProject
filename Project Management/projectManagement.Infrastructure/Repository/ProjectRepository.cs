using Microsoft.EntityFrameworkCore;
using projectManagement.Application.DTO;
using projectManagement.Application.Interfaces;
using projectManagement.Domain.Entities;
using projectManagement.Infrastructure.Context;

namespace projectManagement.Infrastructure.Repository;

public class ProjectRepository : IProjectRepository
{
    private readonly ProjectManagementContext _context;
    public ProjectRepository(ProjectManagementContext context)
    {
        _context = context;
    }

    public async Task<(FilterDto filters, List<Project> projects)> GetProjectsAsync(FilterDto filter)
    {
        List<Project> query = new();
        if (string.IsNullOrEmpty(filter.Role) && (filter.UserId == 0 || filter.UserId == null))
        {
            query = await _context.Projects.Where(p => (p.IsDeleted == false || p.IsDeleted == null) && (string.IsNullOrEmpty(filter.Status) || p.Status == filter.Status)).ToListAsync();
        }
        else
        {
            query = await _context.UserProjectMappings.Where(u => u.Project.IsDeleted == false && (string.IsNullOrEmpty(filter.Role) || u.Role == filter.Role) &&
         (string.IsNullOrEmpty(filter.Status) || u.Project.Status == filter.Status) &&
         (filter.UserId == 0 || filter.UserId == null || filter.UserId == u.UserId)).Select(u => new Project
         {
             Id = u.Project.Id,
             Name = u.Project.Name,
             Description = u.Project.Description,
             Status = u.Project.Status,
             CreatedBy = u.Project.CreatedBy,
             CreatedDate = u.Project.CreatedDate,
             UpdatedDate = u.Project.UpdatedDate,
             IsDeleted = u.Project.IsDeleted
         }).ToListAsync();
        }
        List<Project> distinctProjects = query.DistinctBy(i => i.Name).OrderBy(u => u.Id).ToList();
        filter.TotalCount = distinctProjects.Count;
        List<Project> projects = distinctProjects.Skip((int)((filter.PageIndex - 1) * filter.PageSize)).Take((int)filter.PageSize).ToList();
        return (filter, projects);
    }

    public async Task CreateAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
    }

    public async Task UpdateAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<UserProjectMapping?> GetUserProjectMappingAsync(int userId, int projectId)
    {
        return await _context.UserProjectMappings.FirstOrDefaultAsync(up => up.UserId == userId && up.ProjectId == projectId && up.IsDeleted == false);
    }
}
