using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NBApp.Migrations
{
    /// <inheritdoc />
    public partial class ShippingAddressChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "ShippingAddresses");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "ShippingAddresses");

            migrationBuilder.AddColumn<int>(
                name: "SuburbID",
                table: "ShippingAddresses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityID);
                });

            migrationBuilder.CreateTable(
                name: "Suburbs",
                columns: table => new
                {
                    SuburbID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SuburbName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CityID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suburbs", x => x.SuburbID);
                    table.ForeignKey(
                        name: "FK_Suburbs_Cities_CityID",
                        column: x => x.CityID,
                        principalTable: "Cities",
                        principalColumn: "CityID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_SuburbID",
                table: "ShippingAddresses",
                column: "SuburbID");

            migrationBuilder.CreateIndex(
                name: "IX_Suburbs_CityID",
                table: "Suburbs",
                column: "CityID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingAddresses_Suburbs_SuburbID",
                table: "ShippingAddresses",
                column: "SuburbID",
                principalTable: "Suburbs",
                principalColumn: "SuburbID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_Suburbs_SuburbID",
                table: "ShippingAddresses");

            migrationBuilder.DropTable(
                name: "Suburbs");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_ShippingAddresses_SuburbID",
                table: "ShippingAddresses");

            migrationBuilder.DropColumn(
                name: "SuburbID",
                table: "ShippingAddresses");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ShippingAddresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "ShippingAddresses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
