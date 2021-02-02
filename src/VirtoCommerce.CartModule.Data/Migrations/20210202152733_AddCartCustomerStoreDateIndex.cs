using Microsoft.EntityFrameworkCore.Migrations;

namespace VirtoCommerce.CartModule.Data.Migrations
{
    public partial class AddCartCustomerStoreDateIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE INDEX [IX_CustomerId_StoreId_Date] ON [Cart]([CustomerId] ASC, [StoreId] ASC, [CreatedDate] DESC)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerId_StoreId_Date",
                table: "Cart");
        }
    }
}
