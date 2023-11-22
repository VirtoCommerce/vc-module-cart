using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.SqlServer.Migrations
{
    public partial class ClearWishlistOrganizationId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Cart SET OrganizationId = null WHERE Type = 'Wishlist'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
