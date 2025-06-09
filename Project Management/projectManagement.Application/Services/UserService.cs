
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using projectManagement.Application.DTO;
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

    public async Task<object> GetAllUsersAsync(long? pageIndex, long? pageSize, long? totalCount, long? pageNumber)
    {
        (long Count, List<User> users) = await _userRepository.GetAllAsync(pageIndex, pageSize, totalCount, pageNumber);
        var result = new
        {
            paging = new
            {
                pageIndex = pageIndex ?? 1,
                pageSize = pageSize ?? 10,
                totalCount = Count,
                pageNumber = pageNumber ?? 1
            },
            users = _mapper.Map<List<UserDto>>(users)
        };
        return result;
    }

    public async Task<int> UpdateUser(API.Models.User user)
    {
        User? existingUser = await _userRepository.FindByIdAsync(user.Id);
        if (existingUser == null)
        {
            return 404; // Not Found
        }
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.UserName = user.UserName;
        existingUser.Role = user.Role;
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

    public async Task<int> CreateUser(API.Models.User user)
    {
        User newUser = _mapper.Map<User>(user);
        newUser.IsDeleted = false;
        newUser.Password = "DefaultPassword"; // Set a default password or handle it as needed
        try
        {
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
