using Microsoft.EntityFrameworkCore.Migrations;
using VirtoCommerce.Platform.Data.PostgreSql.Extensions;

#nullable disable

namespace VirtoCommerce.CartModule.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseInsensitiveCollations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateCaseInsensitiveCollationIfNotExists();

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationName",
                table: "Cart",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                collation: "case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cart",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                collation: "case_insensitive",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrganizationName",
                table: "Cart",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldCollation: "case_insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cart",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldCollation: "case_insensitive");
        }
    }
}
