using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projectManagement.Application.Interfaces;
using System;
using projectManagement.API.Controllers;
using System.ComponentModel.DataAnnotations;
using projectManagement.Application.DTO;
using projectManagement.Application.StatusCodes;

namespace projectManagement.API.Implementations;

/// <summary>
///  User API Implementation
/// </summary>
public class User : UserApiController
{
    private readonly IUserService _userService;
    private readonly Enums _statusCode = new();

    /// <summary>
    /// Constructor for User API Implementation
    /// Dependency Injection for IUserService
    /// </summary>
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
        if (user.Id != 0)
        {
            return BadRequest(new { message = "User ID must be zero for creation" });
        }
        int statusCode = await _userService.CreateUser(user);
        return statusCode switch
        {
            201 => Ok(new { message = "User created successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            _ => StatusCode(500, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion

    #region Delete User

    /// <summary>
    /// Delete User
    /// </summary>
    /// <param name="id">ID of user to delete</param>
    /// <response code="200">User deleted successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="404">User not found</response>
    /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> DeleteUser([FromRoute(Name = "id"), Required] long id)
    {
        int statusCode = await _userService.DeleteUser((int)id);
        return statusCode switch
        {
            200 => Ok(new { message = "User deleted successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            404 => NotFound(new { message = "User not found" }),
            _ => StatusCode(0, new { message = "Unexpected error occurred" }),
        };
    }

    #endregion

    #region Find User By Id

    /// <summary>
    /// Find User by Id
    /// </summary>
    /// <param name="id">ID of user to find</param>
    /// <response code="200">User found successfully</response>
    /// <response code="400">Invalid user ID</response>
    /// <response code="404">User not found</response>
    /// <response code="0">Unexpected error</response>
    public override async Task<IActionResult> FindUserById([FromRoute(Name = "id"), Required] long id)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "Invalid user ID" });
        }
        UserDto userData = await _userService.FindUserById((int)id);
        if (userData == null)
        {
            return NotFound(new { message = "User not found" });
        }
        return Ok(userData);
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
            200 => Ok(new { message = "User updated successfully" }),
            400 => BadRequest(new { message = "Bad Request" }),
            404 => NotFound(new { message = "User not found" }),
            _ => StatusCode(500, new { message = "Unexpected error occurred" }),
        };

    }

    #endregion

    #region Users List

    /// <summary>
    /// Get all users
    /// </summary>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="totalCount">Total count</param>
    /// <param name="pageNumber">Page number</param>
    /// <response code="200">Returns a list of users</response>
    /// <response code="400">Bad Request</response>
    public override async Task<IActionResult> GetUsers(long? pageIndex, long? pageSize, long? totalCount, long? pageNumber)
    {
        try
        {
            var response = await _userService.GetAllUsersAsync(pageIndex ?? 1, pageSize ?? 10, pageNumber ?? 1);
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

