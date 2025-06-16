using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using projectManagement.Infrastructure.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace projectManagement.IntegrationTest
{
    public class CustomDatabase<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        // protected override void ConfigureWebHost(IWebHostBuilder builder)
        // {
        //     builder.ConfigureServices(services =>
        //     {
        //         // Remove the existing DbContext registration
        //         var descriptor = services.SingleOrDefault(
        //             d => d.ServiceType == typeof(DbContextOptions<ProjectManagementContext>));

        //         if (descriptor != null)
        //         {
        //             services.Remove(descriptor);
        //         }

        //         // Add a new DbContext with an in-memory database
        //         services.AddDbContext<ProjectManagementContext>(options =>
        //         {
        //             options.UseInMemoryDatabase("TestDatabase");
        //         });

        //         // Ensure the database is created
        //         using (var scope = services.BuildServiceProvider().CreateScope())
        //         {
        //             var db = scope.ServiceProvider.GetRequiredService<ProjectManagementContext>();
        //             // db.Database.EnsureCreated();
        //         }
        //     });
        // }
    }
}
