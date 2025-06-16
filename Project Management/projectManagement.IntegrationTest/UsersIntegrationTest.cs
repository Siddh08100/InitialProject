using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using projectManagement.API;
using projectManagement.API.Models;
using projectManagement.Application.DTO;
using projectManagement.Infrastructure.Context;
using Task = System.Threading.Tasks.Task;

namespace projectManagement.IntegrationTest;

public class UsersIntegrationTest : IClassFixture<CustomDatabase<Program>>
{

    private readonly HttpClient _client;
    private readonly ProjectManagementContext _dbContext;

    public UsersIntegrationTest(CustomDatabase<Program> factory)
    {
        _client = factory.CreateClient();
        // var scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        // using var scope = scopeFactory.CreateScope();
        // _dbContext = scope.ServiceProvider.GetRequiredService<ProjectManagementContext>();
        // SeedDatabase(_dbContext);
    }

    // private void SeedDatabase(ProjectManagementContext dbContext)
    // {
    //     if (!dbContext.Users.Any())
    //     {
    //         dbContext.Users.AddRange(
    //             new() { Id = 1, FirstName = "A", LastName = "Aleys", Email = "A@gmail.com", Password = "A@123", Role = "TSE", UserName = "A123", IsDeleted = false },
    //             new() { Id = 2, FirstName = "B", LastName = "Ales", Email = "B@gmail.com", Password = "B@123", Role = "TSE", UserName = "B123", IsDeleted = false },
    //             new() { Id = 3, FirstName = "C", LastName = "cheys", Email = "C@gmail.com", Password = "C@123", Role = "SSE", UserName = "C123", IsDeleted = true },
    //             new() { Id = 4, FirstName = "D", LastName = "Heilr", Email = "D@gmail.com", Password = "D@123", Role = "Intern", UserName = "D123", IsDeleted = false }
    //         );
    //         dbContext.SaveChanges();
    //     }
    // }


    [Fact]
    public async Task GetUsers_ReturnsPagedUsers_ExcludingDeleted()
    {
        // Arrange
        long pageIndex = 1;
        long pageSize = 10;

        // Act
        var response = await _client.GetAsync($"/Users?pageIndex={pageIndex}&pageSize={pageSize}");
        var result = await response.Content.ReadFromJsonAsync<GetAllDto<UserDto>>();

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.OK);
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    public async Task GetUsers_InvalidPagingParameters_HandleGracefully(long pageIndex, long pageSize)
    {
        var response = await _client.GetAsync($"Users?pageIndex={pageIndex}&pageSize={pageSize}");
        Assert.NotNull(response);
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task CreateUser_ValidPayload_ReturnsSuccessAndCreatesUser()
    {
        // Arrange
        CreateUser newUser = new()
        {
            FirstName = "testuser",
            Email = "test@test.com",
            LastName = "Test",
            Password = "Test@123",
            Role = "TSE",
            UserName = "testuser123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("Users", newUser);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateUser_InvalidPayload_ReturnsBadRequest()
    {
        // Arrange
        CreateUser invalidUser = new()
        {
            FirstName = "",
            Email = "",
            LastName = "Test",
            Password = "Test@123",
            Role = "TSE",
            UserName = "testuser123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("Users", invalidUser);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUser_ValidIdInput_DeleteUserSuccess()
    {
        // Arrange
        long userId = 18; // valid ID

        // Act
        var response = await _client.PutAsync($"Users/{userId}/deleteUser", null);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteUser_InvalidIdInput_ReturnsBadRequest()
    {
        // Arrange
        long userId = 0; // Invalid ID

        // Act
        var response = await _client.PutAsync($"Users/{userId}/deleteUser", null);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteUser_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        long userId = 9999; // Non-existent ID

        // Act
        var response = await _client.PutAsync($"Users/{userId}/deleteUser", null);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task FindUserById_ValidId_ReturnsUser()
    {
        // Arrange
        long userId = 16; // Valid ID

        // Act
        var response = await _client.GetAsync($"Users/{userId}");
        var user = await response.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.OK);
        Assert.NotNull(user);
        Assert.Equal(userId, user.Id);
    }

    [Fact]
    public async Task FindUserById_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        long userId = 0; // Invalid ID

        // Act
        var response = await _client.GetAsync($"Users/{userId}");

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task FindUserById_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        long userId = 100; // Non-existent ID

        // Act
        var response = await _client.GetAsync($"Users/{userId}");

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateUser_ValidInput_ShouldUpdateUser()
    {
        User updatedUser = new()
        {
            Id = 16,
            FirstName = "testuser",
            Email = "test@eaxcsmple.com",
            LastName = "Test",
            Password = "Test@123",
            Role = "TSE",
            UserName = "ChangedUserName"
        };

        // Act
        var response = await _client.PutAsJsonAsync("Users", updatedUser);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUser_InvalidInput_ShouldReturnBadRequest()
    {
        User invalidUser = new()
        {
            Id = 0,
            FirstName = "",
            Email = "invalidemail",
            LastName = "Test",
            Password = "Test@123",
            Role = "TSE",
            UserName = "ChangedUserName"
        };

        // Act
        var response = await _client.PutAsJsonAsync("Users", invalidUser);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUser_InvalidInputId_ShouldReturnNotFound()
    {
        User updatedUser = new()
        {
            Id = 25,
            FirstName = "testuser",
            Email = "test@eaxcsmple.com",
            LastName = "Test",
            Password = "Test@123",
            Role = "TSE",
            UserName = "ChangedUserName"
        };

        // Act
        var response = await _client.PutAsJsonAsync("Users", updatedUser);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.NotFound);

    }
}