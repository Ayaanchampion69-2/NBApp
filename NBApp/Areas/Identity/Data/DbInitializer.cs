/*using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NBApp.Models;

namespace NBApp.Areas.Identity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(NBAppContext context)
        {
            // Ensure the database is created
            //Look for data
            if (context.Categories.Any())
            {
                return;   // DB has been seeded
            }
            var categories = new Category[]
            {
                new Category{Name = "Ice Blocks", Description = "Flavoured frozen treats to beat the heat." },
                new Category{Name = "Ice Creams", Description = "Creamy and delicious frozen desserts." },
            };
            foreach (Category c in categories)
            {
                context.Categories.Add(c);
            }
            context.SaveChanges();

            var products = new Products[]
            {
                new Products{Name = "Mango Ice Block", Price = 1.99m,ImageUrl="https://images.unsplash.com/photo-1625860650806-871900fe2c36?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8M3x8bWFuZ28lMjBpY2UlMjBibG9ja3N8ZW58MHx8MHx8fDA%3D",StockQuantity=100,IsActive=true, CategoryId = 1 },
                new Products{Name = "Chocolate Ice Cream", Price = 3.49m,ImageUrl="https://plus.unsplash.com/premium_photo-1741218406315-92a49a8a6e09?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8Y2hvY28lMjBpY2UlMjBjcmVhbXxlbnwwfHwwfHx8MA%3D%3D",StockQuantity=50,IsActive=true, CategoryId = 2 },
            };
            foreach (Products p in products)
            {
                context.Products.Add(p);
            }
            context.SaveChanges();
        }
    }
}
