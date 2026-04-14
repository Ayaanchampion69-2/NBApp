using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NBApp.Models;

namespace NBApp.Areas.Identity.Data;

public class NBAppContext : IdentityDbContext<NBAppUser>
{
    public NBAppContext(DbContextOptions<NBAppContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

public DbSet<NBApp.Models.Order> Order { get; set; } = default!;
public DbSet<NBApp.Models.OrderItem> OrderItem { get; set; } = default!;
    public DbSet<NBApp.Models.Products> Products { get; set; } = default!;
    public DbSet<NBApp.Models.Category> Categories { get; set; } = default!;

    // seeding
}
