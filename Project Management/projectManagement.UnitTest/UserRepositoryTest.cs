using Microsoft.EntityFrameworkCore;
using projectManagement.Domain.Entities;
using projectManagement.Infrastructure.Context;
using projectManagement.Infrastructure.Repository;

namespace projectManagement.Users;

public class UserRepositoryTest
{
    private DbContextOptions<ProjectManagementContext> _dbContextOptions;

    public UserRepositoryTest()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ProjectManagementContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new ProjectManagementContext(_dbContextOptions);
    }

    // [Fact]
    // public async Task AddUser_ShouldAddUserToDatabase()
    // {
    //     // Arrange
    //     var user = new User { Id = 1, Name = "John" };
    //     var repository = new UserRepository(context);

    //     // Act
    //     await repository.Add(user);
    //     await context.SaveChangesAsync();

    //     // Assert
    //     var addedUser = await context.Users.FindAsync(1);
    //     Assert.NotNull(addedUser);
    //     Assert.Equal("John", addedUser.Name);
    // }

    private ProjectManagementContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ProjectManagementContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var context = new ProjectManagementContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Users.AddRange(new List<User>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "Aleys", Email = "Alice@gmail.com",Password = "Alice@123",Role = "TSE",UserName = "Alice123", IsDeleted = false },
            new() { Id = 2, FirstName = "Bob",  LastName = "Ales", Email = "Bob@gmail.com",Password = "Bob@123",Role = "TSE",UserName = "Bob123", IsDeleted = false },
            new() { Id = 3, FirstName = "Charlie", LastName = "cheys", Email = "Charlie@gmail.com",Password = "Charlie@123",Role = "SSE",UserName = "Charlie123", IsDeleted = true },
            new() { Id = 4, FirstName = "David", LastName = "Heilr", Email = "David@gmail.com",Password = "David@123",Role = "Intern",UserName = "David123", IsDeleted = false }
        });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCorrectPagedUsers()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);
        long pageIndex = 1;
        long pageSize = 2;

        // Act
        var (count, users) = await repository.GetAllAsync(pageIndex, pageSize);

        // Assert
        Assert.Equal(3, count);
        Assert.Equal(2, users.Count);
        Assert.Equal(1, users[0].Id);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);
        int userId = 1;

        // Act
        User? user = await repository.FindByIdAsync(userId);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("Alice", user.FirstName);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);
        int userId = 5;

        // Act
        User? user = await repository.FindByIdAsync(userId);

        // Assert
        Assert.Null(user);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);
        User? userToUpdate = await repository.FindByIdAsync(1);
        userToUpdate.FirstName = "UpdatedAlice";

        // Act
        await repository.UpdateAsync(userToUpdate);
        await context.SaveChangesAsync();

        // Assert
        var updatedUser = await repository.FindByIdAsync(1);
        Assert.NotNull(updatedUser);
        Assert.Equal("UpdatedAlice", updatedUser.FirstName);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_ShouldAddUser()
    {
        //Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);
        User user = new()
        {
            FirstName = "New",
            LastName = "User",
            Email = "NewUser@gmail.com",
            Password = "NewUser@123",
            Role = "Team Lead",
            UserName = "NewUser123",
            IsDeleted = false
        };

        // Act
        await repository.AddAsync(user);

        // Act
        User? addedUser = await repository.FindByIdAsync(user.Id);

        // Assert
        Assert.NotNull(addedUser);
        Assert.Equal("New", addedUser.FirstName);
        Assert.Equal("User", addedUser.LastName);
    }

    [Fact]
    public async Task AddAsync_WithNullUser_ShouldThrowException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);
        User? user = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddAsync(user));
    }

}
