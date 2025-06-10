using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using projectManagement.Domain.Entities;

namespace projectManagement.Infrastructure.Context;

public class ProjectManagementContext : DbContext
{
    public ProjectManagementContext(DbContextOptions<ProjectManagementContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Tasks> Tasks { get; set; }
    public DbSet<UserProjectMapping> UserProjectMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Project>()
            .Property(u => u.CreatedDate)
            .HasDefaultValueSql("now()");


        modelBuilder.Entity<Tasks>()
            .Property(u => u.CreatedDate)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<Tasks>()
            .HasIndex(u => u.CreatedDate);

    }

}
