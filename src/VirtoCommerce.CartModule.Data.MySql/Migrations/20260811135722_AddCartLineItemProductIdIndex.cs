using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.MySql.Migrations
{
    /// <inheritdoc />
    public partial class AddCartLineItemProductIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Unguarded, unlike the SQL Server and PostgreSQL migrations. MySQL has no
            // CREATE INDEX IF NOT EXISTS, and the hand-created index this guards against on the
            // other providers was a SQL Server workaround, so a MySQL installation will not have
            // it. If one does, drop it before upgrading.
            migrationBuilder.CreateIndex(
                name: "IX_CartLineItem_ProductId_IsGift",
                table: "CartLineItem",
                columns: new[] { "ProductId", "IsGift", "ShoppingCartId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartLineItem_ProductId_IsGift",
                table: "CartLineItem");
        }
    }
}
