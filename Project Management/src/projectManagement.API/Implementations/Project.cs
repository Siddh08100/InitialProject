using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projectManagement.API.Controllers;
using projectManagement.API.Models;
using projectManagement.Application.DTO;
using projectManagement.Application.Interfaces;

namespace projectManagement.API.Implementations;

/// <summary>
///  Project API Implementation
/// </summary>
public class Project : ProjectApiController
{
    private readonly IProjectService _projectService;

    /// <summary>
    /// Constructor for Project API Implementation and Dependency Injection
    /// </summary>
    public Project(IProjectService projectService)
    {
        _projectService = projectService;
    }

    #region Create Project

    /// <summary>
    /// Create a new project
    /// </summary>
    /// <param name="project">Project details</param>
    /// <response code="201">Project created successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> CreateProject([FromBody] Models.Project project)
    {
        if (project.Id != 0)
        {
            return BadRequest(new { message = "Project ID must be zero for creation" });
        }
        int statusCode = await _projectService.CreateProject(project);
        return statusCode switch
        {
            201 => Ok(new { message = "Project created successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            _ => StatusCode(0, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion

    #region Delete Project

    /// <summary>
    /// Delete Project
    /// </summary>
    /// <param name="id">ID of project to delete</param>
    /// <response code="200">Project deleted successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">Project not found</response>
    /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> DeleteProject([FromRoute(Name = "id"), Required] long id)
    {
        int statusCode = await _projectService.DeleteProject((int)id);
        return statusCode switch
        {
            200 => Ok(new { message = "Project deleted successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            404 => NotFound(new { message = "Project not found" }),
            _ => StatusCode(0, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion

    #region Get Project

    /// <summary>
    /// Get Project List
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="totalCount"></param>
    /// <param name="pageNumber"></param>
    /// <param name="status"></param>
    /// <param name="userId"></param>
    /// <param name="projectId"></param>
    /// <param name="role"></param>
    /// <response code="200">Returns a list of projects</response>
    /// <response code="0">error occured</response>
    public override async Task<IActionResult> GetProjects([FromQuery(Name = "pageIndex")] long? pageIndex, [FromQuery(Name = "pageSize")] long? pageSize, [FromQuery(Name = "totalCount")] long? totalCount, [FromQuery(Name = "pageNumber")] long? pageNumber, [FromQuery(Name = "status")] string status, [FromQuery(Name = "userId")] long? userId, [FromQuery(Name = "projectId")] long? projectId, [FromQuery(Name = "role")] string role)
    {
        FilterDto filter = new()
        {
            PageIndex = (int?)pageIndex ?? 1,
            PageSize = (int?)pageSize ?? 10,
            TotalCount = (int?)totalCount,
            PageNumber = (int?)pageNumber ?? 1,
            Status = status,
            UserId = (int?)userId,
            ProjectId = (int?)projectId,
            Role = role
        };
        try
        {
            var response = await _projectService.GetProjects(filter);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                ex.Message
            });
        }
    }

    #endregion

    #region Update Project

    /// <summary>
    /// Update Project
    /// </summary>
    /// <param name="project">Project details</param>
    /// /// <response code="200">Project updated successfully</response>
    /// /// <response code="400">Bad Request</response>
    /// /// <response code="404">Project not found</response>
    /// /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> UpdateProject([FromBody] Models.Project project)
    {
        if (project.Id <= 0)
        {
            return BadRequest(new { message = "Project ID must be greater than zero for update" });
        }
        int statusCode = await _projectService.UpdateProject(project);
        return statusCode switch
        {
            200 => Ok(new { message = "Project updated successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            404 => NotFound(new { message = "Project not found" }),
            _ => StatusCode(0, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion

    #region Update User Project Status

    /// <summary>
    /// Update User Project Status
    /// </summary>
    /// <param name="updateUserProjectStatusRequest">Request containing user ID, project ID, and new status</param>
    /// <response code="200">User project status updated successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">User or project not found</response>
    /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> UpdateUserProjectStatus([FromBody] UpdateUserProjectStatusRequest updateUserProjectStatusRequest)
    {
        if (updateUserProjectStatusRequest.ProjectId <= 0 || updateUserProjectStatusRequest.UserId <= 0)
        {
            return BadRequest(new { message = "Invalid user or project ID" });
        }
        int statusCode = await _projectService.UpdateUserProjectStatus(updateUserProjectStatusRequest);
        return statusCode switch
        {
            200 => Ok(new { message = "User project role status updated successfully" }),
            404 => NotFound(new { message = "User or project not found" }),
            _ => StatusCode(0, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion
}
