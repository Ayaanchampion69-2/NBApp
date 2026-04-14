using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NBApp.Migrations
{
    /// <inheritdoc />
    public partial class DBseeded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Flavoured frozen treats to beat the heat.", "Ice Blocks" },
                    { 2, "Creamy and delicious frozen desserts.", "Ice Creams" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "IsActive", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 1, 1, "Refreshing mango-flavored ice block.", "https://images.unsplash.com/photo-1625860650806-871900fe2c36?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8M3x8bWFuZ28lMjBpY2UlMjBibG9ja3N8ZW58MHx8MHx8fDA%3D", true, "Mango Ice Block", 1.99m, 100 },
                    { 2, 2, "Rich and creamy chocolate ice cream.", "https://plus.unsplash.com/premium_photo-1741218406315-92a49a8a6e09?w=600&auto=format&fit=crop&q=60&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8Y2hvY28lMjBpY2UlMjBjcmVhbXxlbnwwfHwwfHx8MA%3D%3D", true, "Chocolate Ice Cream", 3.49m, 50 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2);
        }
    }
}
