using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CartModule.Data.SqlServer.Migrations
{
    public partial class IsRejectedLineitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "CartLineItem",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "CartLineItem");
        }
    }
}
