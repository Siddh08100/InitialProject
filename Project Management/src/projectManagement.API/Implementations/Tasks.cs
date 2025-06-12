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
/// Tasks API Implementation
/// </summary>
public class Tasks : TaskApiController
{
    private readonly ITaskService _taskService;

    /// <summary>
    /// Tasks Controller and Dependency Injection
    /// </summary>
    public Tasks(ITaskService taskService)
    {
        _taskService = taskService;
    }

    #region Create Task

    /// <summary>
    /// Create Task
    /// </summary>
    /// <param name="createTaskRequest">Create Task Request</param>
    /// <response code="201">Task created successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest createTaskRequest)
    {
        int statusCode = await _taskService.CreateTask(createTaskRequest);
        return statusCode switch
        {
            201 => Ok(new { message = "Task created successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            _ => StatusCode(0, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion

    #region Get task by Id

    /// <summary>
    /// Get Task by Id
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <response code="200">Task retrieved successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">Task not found</response>
    public override async Task<IActionResult> GetTaskById([FromRoute(Name = "id"), Required] long id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid Task ID" });
        }
        TasksDto tasks = await _taskService.GetTasksAsync((int)id);
        if (tasks == null)
        {
            return NotFound(new { message = "Task not found" });
        }
        return Ok(tasks);
    }

    #endregion

    #region Get Tasks

    /// <summary>
    /// Get Tasks Lisrt
    /// </summary>
    /// <param name="pageIndex">Page Index</param>
    /// <param name="pageSize">Page Size</param>
    /// <param name="totalCount">Total Count</param>
    /// <param name="pageNumber">Page Number</param>
    /// <param name="status">Status</param>
    /// <param name="userId">User Id</param>
    /// <param name="projectId">Project Id</param>
    /// <returns>List of Tasks</returns>
    public override async Task<IActionResult> GetTasks([FromQuery(Name = "pageIndex")] long? pageIndex, [FromQuery(Name = "pageSize")] long? pageSize,[FromQuery(Name = "status")] string status, [FromQuery(Name = "userId")] long? userId, [FromQuery(Name = "projectId")] long? projectId)
    {
        FilterDto filter = new()
        {
            PageIndex = (int?)pageIndex ?? 1,
            PageSize = (int?)pageSize ?? 10,
            Status = status,
            UserId = (int?)userId,
            ProjectId = (int?)projectId,
        };
        try
        {
            var response = await _taskService.GetTasks(filter);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                ex.Message
            });
        }
        throw new System.NotImplementedException();
    }

    #endregion

    #region Update Task

    /// <summary>
    /// Update Task
    /// </summary>
    /// <param name="updateTaskRequest">Update Task Request</param>
    /// <response code="200">Task updated successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">Task not found</response>
    /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> UpdateTask([FromBody] UpdateTaskRequest updateTaskRequest)
    {
        if (updateTaskRequest.Id <= 0)
        {
            return BadRequest(new { message = "Invalid Task ID" });
        }
        int statusCode = await _taskService.UpdateTaskAsync(updateTaskRequest);
        return statusCode switch
        {
            200 => Ok(new { message = "Task updated successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            404 => NotFound(new { message = "Task not found" }),
            _ => StatusCode(0, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion

}
