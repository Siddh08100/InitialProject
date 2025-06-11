
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using projectManagement.Application.DTO;
using projectManagement.Application.StatusCodes;
using projectManagement.Application.Interfaces;
using projectManagement.Domain.Entities;

namespace projectManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<object> GetAllUsersAsync(long pageIndex, long pageSize, long pageNumber)
    {
        (long totalCount, List<User> users) = await _userRepository.GetAllAsync(pageIndex, pageSize);
        UsersListsDto result = new()
        {
            Paging = new PagingDto
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                PageNumber = pageNumber
            },
            Users = _mapper.Map<List<UserDto>>(users)
        };
        return result;
    }

    public async Task<int> UpdateUser(API.Models.User user)
    {
        if (user.Id == 0)
        {
            return 400; // Bad Request
        }
        User? existingUser = await _userRepository.FindByIdAsync(user.Id);
        if (existingUser?.Id == 0 || existingUser == null)
        {
            return 404; // User Not Found
        }
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.UserName = user.UserName;
        existingUser.Role = user.Role;
        existingUser.Password = user.Password;
        try
        {
            await _userRepository.UpdateAsync(existingUser);
        }
        catch (Exception)
        {
            return 400; // Bad Request
        }
        return 200; // User Updated Successfully
    }

    public async Task<int> CreateUser(API.Models.User user)
    {
        try
        {
            User newUser = _mapper.Map<User>(user);
            await _userRepository.AddAsync(newUser);
        }
        catch (Exception)
        {
            return 400; // Bad Request
        }
        return 201; // Created
    }

    public async Task<UserDto?> FindUserById(int id)
    {
        User? user = await _userRepository.FindByIdAsync(id);
        UserDto newUser = _mapper.Map<UserDto>(user);
        return newUser;
    }

    public async Task<int> DeleteUser(int id)
    {
        if (id <= 0)
        {
            return 400; // Bad Request
        }
        User? existingUser = await _userRepository.FindByIdAsync(id);
        if (existingUser == null)
        {
            return 404; // Not Found
        }
        existingUser.IsDeleted = true;
        try
        {
            await _userRepository.UpdateAsync(existingUser);
        }
        catch (Exception)
        {
            return 400; // Bad Request
        }
        return 200; // OK
    }
}
