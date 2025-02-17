using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.MySql.Migrations
{
    /// <inheritdoc />
    public partial class AddCartConfigurationItemTypeAndText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomText",
                table: "CartConfigurationItem",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CartConfigurationItem",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "Product")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomText",
                table: "CartConfigurationItem");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CartConfigurationItem");
        }
    }
}
