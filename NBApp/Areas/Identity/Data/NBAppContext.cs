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

    /*protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }*/

public DbSet<NBApp.Models.Order> Orders { get; set; } = default!;
public DbSet<NBApp.Models.OrderItem> OrderItem { get; set; } = default!;
    public DbSet<NBApp.Models.Products> Products { get; set; } = default!;
    public DbSet<NBApp.Models.Category> Categories { get; set; } = default!;
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //seed categories
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Ice Blocks", Description = "Flavoured frozen treats to beat the heat." },
            new Category { CategoryId = 2, Name = "Ice Creams", Description = "Creamy and delicious frozen desserts." }
        );
        //seed products
        modelBuilder.Entity<Products>().HasData(
            new Products { ProductId = 1, Name = "Mango Ice Block", Description = "Refreshing mango-flavored ice block.", Price = 1.99m,ImageUrl= "https://images.unsplash.com/photo-1625860650806-871900fe2c36?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8M3x8bWFuZ28lMjBpY2UlMjBibG9ja3N8ZW58MHx8MHx8fDA%3D", StockQuantity=100, IsActive=true, CategoryId = 1 },
            new Products { ProductId = 2, Name = "Chocolate Ice Cream", Description = "Rich and creamy chocolate ice cream.", Price = 3.49m,ImageUrl= "https://plus.unsplash.com/premium_photo-1741218406315-92a49a8a6e09?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8Y2hvY28lMjBpY2UlMjBjcmVhbXxlbnwwfHwwfHx8MA%3D%3D", StockQuantity=50, IsActive=true, CategoryId = 2 }
        );
    }
    
   

}
