using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddCartLineItemProductIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Guarded rather than a plain CreateIndex: installations that hit the wishlist-query
            // regression may have created this index by hand before upgrading, and migrations run
            // during module initialisation -- an unguarded CREATE INDEX would fail on every restart.
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS ""IX_CartLineItem_ProductId_IsGift""
                    ON ""CartLineItem"" (""ProductId"", ""IsGift"", ""ShoppingCartId"");
            ");
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
