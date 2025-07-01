using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Cart_AddPickupLocationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PickupLocationId",
                table: "CartShipment",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupLocationId",
                table: "CartShipment");
        }
    }
}
