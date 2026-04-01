using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.MySql.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrphanedCartAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM `CartAddress` WHERE `ShoppingCartId` IS NULL AND `ShipmentId` IS NULL AND `PaymentId` IS NULL;
                DELETE FROM `CartDiscount` WHERE `ShoppingCartId` IS NULL AND `ShipmentId` IS NULL AND `LineItemId` IS NULL AND `PaymentId` IS NULL;
                DELETE FROM `CartTaxDetail` WHERE `ShoppingCartId` IS NULL AND `ShipmentId` IS NULL AND `LineItemId` IS NULL AND `PaymentId` IS NULL;
                DELETE FROM `CartDynamicPropertyObjectValue` WHERE `ShoppingCartId` IS NULL AND `ShipmentId` IS NULL AND `LineItemId` IS NULL AND `PaymentId` IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
