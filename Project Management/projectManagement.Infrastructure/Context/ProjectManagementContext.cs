using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace projectManagement.Infrastructure.Context;

public class ProjectManagementContext : DbContext
{
    public ProjectManagementContext(DbContextOptions<ProjectManagementContext> options) : base(options) { }

    public DbSet<Domain.Entities.User> Users { get; set; }
    public DbSet<Domain.Entities.Project> Projects { get; set; }
    public DbSet<Domain.Entities.Tasks> Tasks { get; set; }
    public DbSet<Domain.Entities.UserProjectMapping> UserProjectMappings { get; set; }

}
