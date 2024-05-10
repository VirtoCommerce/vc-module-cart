using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddCartIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cart_Type_IsDeleted",
                table: "Cart",
                columns: new[] { "Type", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_CartLineItem_ProductId",
                table: "CartLineItem",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Cart_Type_IsDeleted", table: "Cart");

            migrationBuilder.DropIndex(name: "IX_CartLineItem_ProductId", table: "CartLineItem");
        }
    }
}
