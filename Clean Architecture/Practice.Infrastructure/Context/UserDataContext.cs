using Microsoft.EntityFrameworkCore;
using Practice.Domain.Entities;

namespace Practice.Infrastructure.Context;

public class UserDataContext : DbContext
{
    public UserDataContext(DbContextOptions<UserDataContext> options) : base(options) 
    {
    }
    
    public DbSet<User> Users { get; set; }
}

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // => optionsBuilder.UseNpgsql("Host=localhost;port=5432;Database=UserData;Username=postgres;Password=Tatva@123");