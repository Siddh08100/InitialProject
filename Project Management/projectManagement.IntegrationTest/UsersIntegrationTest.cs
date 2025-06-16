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
    //             new() { Id = 1, FirstName = "Alice", LastName = "Aleys", Email = "Alice@gmail.com", Password = "Alice@123", Role = "TSE", UserName = "Alice123", IsDeleted = false },
    //             new() { Id = 2, FirstName = "Bob", LastName = "Ales", Email = "Bob@gmail.com", Password = "Bob@123", Role = "TSE", UserName = "Bob123", IsDeleted = false },
    //             new() { Id = 3, FirstName = "Charlie", LastName = "cheys", Email = "Charlie@gmail.com", Password = "Charlie@123", Role = "SSE", UserName = "Charlie123", IsDeleted = true },
    //             new() { Id = 4, FirstName = "David", LastName = "Heilr", Email = "David@gmail.com", Password = "David@123", Role = "Intern", UserName = "David123", IsDeleted = false }
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
        long userId = 1; // Assuming this user exists in the seeded data

        // Act
        var response = await _client.PutAsync($"Users/{userId}/deleteUser", null);

        // Assert
        Assert.Equal(response.StatusCode, HttpStatusCode.NoContent);
    }
}