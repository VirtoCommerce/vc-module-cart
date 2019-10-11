using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CartModule.Data.Migrations
{
    public partial class MissedChangesFromSecondVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartLineItem_LineItemId",
                table: "CartDynamicPropertyObjectValue");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartPayment_PaymentId",
                table: "CartDynamicPropertyObjectValue");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartShipment_ShipmentId",
                table: "CartDynamicPropertyObjectValue");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_Cart_ShoppingCartId",
                table: "CartDynamicPropertyObjectValue");

            migrationBuilder.AddColumn<string>(
                name: "LineItemEntityId",
                table: "CartShipmentItem",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FulfillmentCenterName",
                table: "CartShipment",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FulfillmentCenterName",
                table: "CartLineItem",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FulfilmentCenterId",
                table: "CartLineItem",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Cart",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CartShipmentItem_LineItemEntityId",
                table: "CartShipmentItem",
                column: "LineItemEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartLineItem_LineItemId",
                table: "CartDynamicPropertyObjectValue",
                column: "LineItemId",
                principalTable: "CartLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartPayment_PaymentId",
                table: "CartDynamicPropertyObjectValue",
                column: "PaymentId",
                principalTable: "CartPayment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartShipment_ShipmentId",
                table: "CartDynamicPropertyObjectValue",
                column: "ShipmentId",
                principalTable: "CartShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_Cart_ShoppingCartId",
                table: "CartDynamicPropertyObjectValue",
                column: "ShoppingCartId",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartShipmentItem_CartLineItem_LineItemEntityId",
                table: "CartShipmentItem",
                column: "LineItemEntityId",
                principalTable: "CartLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartLineItem_LineItemId",
                table: "CartDynamicPropertyObjectValue");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartPayment_PaymentId",
                table: "CartDynamicPropertyObjectValue");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartShipment_ShipmentId",
                table: "CartDynamicPropertyObjectValue");

            migrationBuilder.DropForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_Cart_ShoppingCartId",
                table: "CartDynamicPropertyObjectValue");

            migrationBuilder.DropForeignKey(
                name: "FK_CartShipmentItem_CartLineItem_LineItemEntityId",
                table: "CartShipmentItem");

            migrationBuilder.DropIndex(
                name: "IX_CartShipmentItem_LineItemEntityId",
                table: "CartShipmentItem");

            migrationBuilder.DropColumn(
                name: "LineItemEntityId",
                table: "CartShipmentItem");

            migrationBuilder.DropColumn(
                name: "FulfillmentCenterName",
                table: "CartShipment");

            migrationBuilder.DropColumn(
                name: "FulfillmentCenterName",
                table: "CartLineItem");

            migrationBuilder.DropColumn(
                name: "FulfilmentCenterId",
                table: "CartLineItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Cart");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartLineItem_LineItemId",
                table: "CartDynamicPropertyObjectValue",
                column: "LineItemId",
                principalTable: "CartLineItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartPayment_PaymentId",
                table: "CartDynamicPropertyObjectValue",
                column: "PaymentId",
                principalTable: "CartPayment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_CartShipment_ShipmentId",
                table: "CartDynamicPropertyObjectValue",
                column: "ShipmentId",
                principalTable: "CartShipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDynamicPropertyObjectValue_Cart_ShoppingCartId",
                table: "CartDynamicPropertyObjectValue",
                column: "ShoppingCartId",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
