using Microsoft.EntityFrameworkCore;
using projectManagement.Infrastructure.Context;

namespace projectManagement.UnitTest;

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


}
