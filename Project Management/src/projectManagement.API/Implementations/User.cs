using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projectManagement.Application.DTO;
using System;
using projectManagement.API.Models;
using projectManagement.API.Controllers;
using System.Linq;
using projectManagement.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace projectManagement.API.Implementations;

public class User : UserApiController
{
    private readonly IUserService _userService;
    public User(IUserService userService)
    {
        _userService = userService;
    }

    #region Create User

    /// <summary>
    /// Create User
    /// </summary>
    /// <param name="user"></param>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> CreateUser([FromBody] Models.User user)
    {
        int statusCode = await _userService.CreateUser(user);
        return statusCode switch
        {
            200 => Ok(new { message = "User updated successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            _ => StatusCode(500, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion

    #region Delete User

    public override async Task<IActionResult> DeleteUser([FromRoute(Name = "id"), Required] long id)
    {
        int statusCode = await _userService.DeleteUser((int)id);
        return statusCode switch
        {
            200 => Ok(new { message = "User deleted successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            404 => NotFound(new { message = "User not found" }),
            _ => StatusCode(500, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion

    #region Find User By Id

    public override async Task<IActionResult> FindUserById([FromRoute(Name = "id"), Required] long id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid user ID" });
        }
        UserDto statusCode = await _userService.FindUserById((int)id);
        if (statusCode == null)
        {
            return NotFound(new { message = "User not found" });
        }
        return Ok(statusCode);
    }

    #endregion 

    #region Update User

    /// <summary>
    /// Update User
    /// </summary>
    /// <param name="user"></param>
    /// <response code="200">User updated successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">User not found</response>
    /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> UpdateUser([FromBody] Models.User user)
    {
        int statusCode = await _userService.UpdateUser(user);
        return statusCode switch
        {
            201 => Ok(new { message = "User updated successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            404 => NotFound(new { message = "User not found" }),
            _ => StatusCode(500, new { message = "Unexpected error occurred" }),
        };

    }

    #endregion

    #region Users List

    /// <summary>
    /// Get all users
    /// /// </summary>
    /// /// <param name="pageIndex">Page index</param>
    /// /// <param name="pageSize">Page size</param>
    /// /// <param name="totalCount">Total count</param>
    /// /// <param name="pageNumber">Page number</param>
    /// /// <response code="200">Returns a list of users</response>
    /// /// <response code="400">Bad Request</response>
    public override async Task<IActionResult> Users(long? pageIndex, long? pageSize, long? totalCount, long? pageNumber)
    {
        try
        {
            var response = await _userService.GetAllUsersAsync(pageIndex ?? 1, pageSize ?? 10, totalCount, pageNumber ?? 1);
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
}
