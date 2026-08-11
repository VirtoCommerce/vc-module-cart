using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddCartLineItemProductIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Guarded rather than a plain CreateIndex. Installations that hit the wishlist-query
            // regression will have created this index by hand ahead of upgrading, and an
            // unguarded CREATE INDEX fails with "There is already an object named ...". Migrations
            // run during module initialisation, so that failure takes the module down and repeats
            // on every restart.
            //
            // A hand-made (ProductId, IsGift) INCLUDE (ShoppingCartId) is equivalent for the query
            // this serves, so an existing index of either shape is left alone.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes
                               WHERE name = 'IX_CartLineItem_ProductId_IsGift'
                                 AND object_id = OBJECT_ID('dbo.CartLineItem'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_CartLineItem_ProductId_IsGift]
                        ON [CartLineItem] ([ProductId], [IsGift], [ShoppingCartId]);
                END
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
