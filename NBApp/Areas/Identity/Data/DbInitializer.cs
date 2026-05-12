using NBApp.Models;

namespace NBApp.Areas.Identity.Data
{
    public static class DbInitializer
    {
        public static void Initialize(NBAppContext context)
        {
            

            if (context.Products.Any())
            {
                return;
            }
            var categories = new Category[]
            {
                new Category
                {
                    Name = "Ice Cream",
                    Description = "We've had ice cream before."
                }
            };
            foreach (Category c in categories)
            {
                context.Categories.Add(c);
            }
            context.SaveChanges();

            var products = new Products[]
            {
                new Products

                {
                Name = "Sample Product 1",
                Description = "This is a sample product 1.",
                Price = 19.99m,
                ImageUrl = "/Products/B&J.jpg",
                StockQuantity = 69,
                CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 2",
                    Description = "This is a sample product 2.",
                    Price = 19.99m,
                    ImageUrl = "/Products/BlueBell.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 3",
                    Description = "This is a sample product 3.",
                    Price = 19.99m,
                    ImageUrl = "/Products/ChocFrozenYoghurt.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 4",
                    Description = "This is a sample product 4.",
                    Price = 19.99m,
                    ImageUrl = "/Products/Dreyer.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 5",
                    Description = "This is a sample product 5.",
                    Price = 19.99m,
                    ImageUrl = "/Products/FrozenCoffeeIce.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 6",
                    Description = "This is a sample product 6.",
                    Price = 19.99m,
                    ImageUrl = "/Products/Gelato.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 7",
                    Description = "This is a sample product 7.",
                    Price = 19.99m,
                    ImageUrl = "/Products/HazelIceCream.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 8",
                    Description = "This is a sample product 8.",
                    Price = 19.99m,
                    ImageUrl = "/Products/KitKatIce.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 9",
                    Description = "This is a sample product 9.",
                    Price = 19.99m,
                    ImageUrl = "/Products/MagnumRipOff.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 10",
                    Description = "This is a sample product 10.",
                    Price = 19.99m,
                    ImageUrl = "/Products/MatchaIceCream.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 11",
                    Description = "This is a sample product 11.",
                    Price = 19.99m,
                    ImageUrl = "/Products/Popsicle.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 12",
                    Description = "This is a sample product 12.",
                    Price = 19.99m,
                    ImageUrl = "/Products/SnowCone.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 13",
                    Description = "This is a sample product 13.",
                    Price = 19.99m,
                    ImageUrl = "/Products/Chococream.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                },
                new Products
                {
                    Name = "Sample Product 14",
                    Description = "This is a sample product 14.",
                    Price = 19.99m,
                    ImageUrl = "/Products/StrawberryIceCream.jpg",
                    StockQuantity = 69,
                    CategoryId = 1
                }
            };
            foreach (Products p in products)
            {
                context.Products.Add(p);

            }
           context.SaveChanges();
        }
    }
}

