using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddLineItemsCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LineItemsCount",
                table: "Cart",
                type: "integer",
            nullable: false,
            defaultValue: 0);

            migrationBuilder.Sql("UPDATE \"Cart\" SET \"LineItemsCount\" = (SELECT COUNT(\"Id\") FROM \"CartLineItem\" WHERE \"Cart\".\"Id\" = \"CartLineItem\".\"ShoppingCartId\")");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineItemsCount",
                table: "Cart");
        }
    }
}
