using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.PostgreSql.Migrations
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
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CartConfigurationItem",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "Product");
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
