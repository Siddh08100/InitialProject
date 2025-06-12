using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Moq;
using projectManagement.API.Implementations;
using projectManagement.Application.DTO;
using projectManagement.Application.Interfaces;
using projectManagement.Application.Services;

namespace projectManagement.Users;

public class UserServiceTest
{
    readonly User _userApi;
    readonly UserService _userService;
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();

    public UserServiceTest()
    {
        _userApi = new User(_mockUserService.Object);
        _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetUsers_WithValidInputs_ShouldWorkProperly()
    {
        // Act
        var users = new List<UserDto>();
        long pageIndex = 1;
        long pageNumber = 1;
        long pageSize = 10;

        _mockUserService.Setup(us => us.GetAllUsersAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(users);

        var result = await _userApi.GetUsers(pageIndex, pageSize);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAllUsersAsync_WithValidInputs_ShouldWorkProperly()
    {
        // Arrange
        long pageIndex = 1;
        long pageSize = 10;
        List<UserDto> userDtos = new()
        {
            new UserDto
            {
                Id = 2,
                FirstName = "Siddh",
                LastName = "Shah",
                UserName = "Siddh@301812",
                Email = "shahsiddh08@gmail.com",
                Role = "Intern",
                CreatedAt = DateTime.Parse("0001-01-01T00:00:00"),
                UpdatedAt = DateTime.Parse("0001-01-01T00:00:00")
            },
        };
        List<Domain.Entities.User> user = new()
        {
            new Domain.Entities.User
            {
                Id = 2,
                FirstName = "Siddh",
                LastName = "Shah",
                UserName = "Siddh@301812",
                Email = "shahsiddh08@gmail.com",
                Role = "Intern",
                Password = "Siddh@123",
                IsDeleted = false
            },
        };
        var expectedUsers = new
        {
            Paging = new
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = user.Count,
                PageNumber = (int)Math.Ceiling((decimal)((double)user.Count / pageSize))
            },
            Users = userDtos
        };

        // Act
        _mockUserRepository.Setup(us => us.GetAllAsync(pageIndex, pageSize)).ReturnsAsync((user.Count, user));
        _mockMapper.Setup(m => m.Map<List<UserDto>>(user)).Returns(userDtos);
        GetAllDto<UserDto>? result = await _userService.GetAllUsersAsync(pageIndex, pageSize) as GetAllDto<UserDto>;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUsers.Paging.PageIndex, result.Paging.PageIndex);
        Assert.Equal(expectedUsers.Paging.PageSize, result.Paging.PageSize);
        Assert.Equal(expectedUsers.Paging.TotalCount, result.Paging.TotalCount);
        Assert.Equal(expectedUsers.Paging.PageNumber, result.Paging.PageNumber);
        Assert.Equal(expectedUsers.Users.Count, result.List.Count);
        for (int i = 0; i < expectedUsers.Users.Count; i++)
        {
            Assert.Equal(expectedUsers.Users[i].Id, result.List[i].Id);
            Assert.Equal(expectedUsers.Users[i].FirstName, result.List[i].FirstName);
            Assert.Equal(expectedUsers.Users[i].LastName, result.List[i].LastName);
            Assert.Equal(expectedUsers.Users[i].UserName, result.List[i].UserName);
            Assert.Equal(expectedUsers.Users[i].Email, result.List[i].Email);
            Assert.Equal(expectedUsers.Users[i].Role, result.List[i].Role);
        }
    }

    [Fact]
    public async Task UpdateUser_WithValidInput_ShouldUpdateUser()
    {
        // Arrange
        Domain.Entities.User user = new()
        {
            Id = 2,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };
        API.Models.User ModelsUser = new()
        {
            Id = 2,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };

        // Act
        _mockUserRepository.Setup(r => r.FindByIdAsync(ModelsUser.Id)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>())).Returns(Task.CompletedTask);

        var result = await _userService.UpdateUser(ModelsUser);

        // Assert
        Assert.Equal(200, result);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<Domain.Entities.User>(
            u => u.FirstName == ModelsUser.FirstName &&
                 u.LastName == ModelsUser.LastName &&
                 u.Email == ModelsUser.Email &&
                 u.UserName == ModelsUser.UserName &&
                 u.Role == ModelsUser.Role &&
                 u.Password == ModelsUser.Password)), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WithInValidUserId_ShouldReturnNotFound()
    {
        API.Models.User ModelsUser = new()
        {
            Id = 5,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };

        // Act
        _mockUserRepository.Setup(r => r.FindByIdAsync(ModelsUser.Id)).ReturnsAsync((Domain.Entities.User?)null);

        var result = await _userService.UpdateUser(ModelsUser);

        // Assert
        Assert.Equal(404, result);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUser_WithUserIdIsZero_ShouldReturnBadRequest()
    {
        API.Models.User ModelsUser = new()
        {
            Id = 0,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };
        // Act
        var result = await _userService.UpdateUser(ModelsUser);

        // Assert
        Assert.Equal(400, result);
        _mockUserRepository.Verify(r => r.FindByIdAsync(It.IsAny<int>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUser_WhenUpdateAsyncThrowsException_ShouldReturnBadRequest()
    {
        // Arrange
        Domain.Entities.User existingUser = new()
        {
            Id = 2,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };

        API.Models.User inputUser = new()
        {
            Id = 2,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "SSE",
            Password = "Siddh@123"
        };

        // Mock FindByIdAsync to return a valid user
        _mockUserRepository.Setup(r => r.FindByIdAsync(inputUser.Id)).ReturnsAsync(existingUser);

        // Mock UpdateAsync to throw an exception
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>())).ThrowsAsync(new Exception("Database update failed"));

        // Act
        int result = await _userService.UpdateUser(inputUser);

        // Assert
        Assert.Equal(400, result); // Exception result in Bad Request
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Once);

    }

    [Fact]
    public async Task CreateUser_WithValidInput_ShouldCreateUser()
    {
        //Arrange
        API.Models.CreateUser NewUserModel = new()
        {
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };

        Domain.Entities.User user = new()
        {
            Id = 2,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };

        //Act
        _mockMapper.Setup(m => m.Map<Domain.Entities.User>(NewUserModel)).Returns(user);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.User>())).Returns(Task.CompletedTask);

        int result = await _userService.CreateUser(NewUserModel);


        // Assert
        Assert.Equal(201, result);
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<Domain.Entities.User>(
            u => u.FirstName == NewUserModel.FirstName &&
                 u.LastName == NewUserModel.LastName &&
                 u.Email == NewUserModel.Email &&
                 u.UserName == NewUserModel.UserName &&
                 u.Role == NewUserModel.Role &&
                 u.Password == NewUserModel.Password)), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithAddAsyncThrowsException_ShouldReturnBadRequest()
    {
        API.Models.CreateUser NewUserModel = new()
        {
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };

        Domain.Entities.User user = new()
        {
            Id = 0,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };

        //Act
        _mockMapper.Setup(m => m.Map<Domain.Entities.User>(NewUserModel)).Returns(user);
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.User>())).ThrowsAsync(new Exception("Database update failed"));

        int result = await _userService.CreateUser(NewUserModel);


        // Assert
        Assert.Equal(400, result);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithInValidMapping_ShouldThrowError()
    {
        //Arrange
        API.Models.CreateUser NewUserModel = new()
        {
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };

        _mockMapper.Setup(m => m.Map<Domain.Entities.User>(It.IsAny<API.Models.CreateUser>()))
           .Throws(new AutoMapperMappingException("Mapping failed"));

        // Act
        var result = await _userService.CreateUser(NewUserModel);

        // Assert
        Assert.Equal(400, result); // Bad request due to mapping error
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task FindUserById_WhenUserExists_ReturnsUserDto()
    {
        // Arrange
        int userId = 2;
        Domain.Entities.User user = new()
        {
            Id = 2,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };

        UserDto expectedDto = new UserDto
        {
            Id = 2,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            CreatedAt = DateTime.Parse("0001-01-01T00:00:00"),
            UpdatedAt = DateTime.Parse("0001-01-01T00:00:00")
        };

        _mockUserRepository.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(expectedDto);

        // Act
        var result = await _userService.FindUserById(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.FirstName, result.FirstName);
    }

    [Fact]
    public async Task FindUserById_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        int userId = 5;
        _mockUserRepository.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync((Domain.Entities.User?)null);

        _mockMapper.Setup(m => m.Map<UserDto>(null)).Returns((UserDto?)null);

        // Act
        var result = await _userService.FindUserById(userId);

        // Assert
        Assert.Null(result);
        _mockMapper.Verify(m => m.Map<UserDto>(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_ReturnsSuccess()
    {
        // Arrange
        int userId = 2;
        Domain.Entities.User user = new()
        {
            Id = 2,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };
        _mockUserRepository.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);

        // Act
        int result = await _userService.DeleteUser(userId);

        // Assert
        Assert.Equal(200, result);
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        int userId = 5;
        _mockUserRepository.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync((Domain.Entities.User?)null);

        // Act
        int result = await _userService.DeleteUser(userId);

        // Assert
        Assert.Equal(404, result);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_WhenUserIdIsZero_ReturnsBadRequest()
    {
        // Arrange
        int userId = 0;

        // Act
        int result = await _userService.DeleteUser(userId);

        // Assert
        Assert.Equal(400, result);
        _mockUserRepository.Verify(r => r.FindByIdAsync(It.IsAny<int>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_WhenUpdateAsyncThrowsException_ReturnsBadRequest()
    {
        int userId = 2;
        Domain.Entities.User user = new()
        {
            Id = 2,
            FirstName = "Siddh",
            LastName = "Shah",
            UserName = "Siddh@301812",
            Email = "shahsiddh08@gmail.com",
            Role = "Intern",
            Password = "Siddh@123"
        };


        _mockUserRepository.Setup(r => r.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>())).ThrowsAsync(new Exception("Database update failed"));

        // Act
        int result = await _userService.DeleteUser(userId);

        // Assert
        Assert.Equal(400, result);
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
    }
}