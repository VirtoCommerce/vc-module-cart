using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CartModule.Data.SqlServer.Migrations
{
    public partial class MissedChangesFromSecondVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "FulfillmentCenterId",
                table: "CartLineItem",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Cart",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FulfillmentCenterName",
                table: "CartShipment");

            migrationBuilder.DropColumn(
                name: "FulfillmentCenterName",
                table: "CartLineItem");

            migrationBuilder.DropColumn(
                name: "FulfillmentCenterId",
                table: "CartLineItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Cart");
        }
    }
}
