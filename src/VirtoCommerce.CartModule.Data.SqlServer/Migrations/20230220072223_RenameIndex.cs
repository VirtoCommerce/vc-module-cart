using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.SqlServer.Migrations
{
    public partial class RenameIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_ObjectType_ObjectId",
                table: "CartDynamicPropertyObjectValue",
                newName: "IX_CartDynamicPropertyObjectValue_ObjectType_ObjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_CartDynamicPropertyObjectValue_ObjectType_ObjectId",
                table: "CartDynamicPropertyObjectValue",
                newName: "IX_ObjectType_ObjectId");
        }
    }
}
