using Microsoft.EntityFrameworkCore;
using SampleApp.Application.Domain.Entities;
using SampleApp.Application.Domain.Enums;

namespace SampleApp.Infrastructure;

public class SampleAppContext : DbContext
{
    public SampleAppContext(DbContextOptions<SampleAppContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        // users
        var users = new List<User>
        {
            new("admin-user", "StrongPassword@123", "980395900", "5036 Tierra Locks Suite 158",
                "Admin User", Role.Admin),
            new("customer-user", "StrongPassword@123", "948019535", "570 Hackett Bridge",
                "Customer User")
        };
        modelBuilder.Entity<User>().HasData(users);
    }
}
