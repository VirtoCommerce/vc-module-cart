using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class Cart_AddSharingSettingTargetsAndMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "CartSharingSetting",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CartSharingSettingTarget",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CartSharingSettingId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    SharedWithId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ModifiedBy = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartSharingSettingTarget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartSharingSettingTarget_CartSharingSetting_CartSharingSett~",
                        column: x => x.CartSharingSettingId,
                        principalTable: "CartSharingSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartSharingSettingTarget_CartSharingSettingId",
                table: "CartSharingSettingTarget",
                column: "CartSharingSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_CartSharingSettingTarget_SharedWithId",
                table: "CartSharingSettingTarget",
                column: "SharedWithId");

            migrationBuilder.Sql(@"
                INSERT INTO ""CartSharingSettingTarget"" (""Id"", ""CartSharingSettingId"", ""SharedWithId"", ""CreatedDate"", ""ModifiedDate"", ""CreatedBy"", ""ModifiedBy"")
                SELECT REPLACE(gen_random_uuid()::text, '-', ''), ""Id"", ""SharedWithId"", ""CreatedDate"", ""ModifiedDate"", ""CreatedBy"", ""ModifiedBy""
                FROM ""CartSharingSetting""
                WHERE ""SharedWithId"" IS NOT NULL;
            ");

            migrationBuilder.DropColumn(
                name: "SharedWithId",
                table: "CartSharingSetting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SharedWithId",
                table: "CartSharingSetting",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ""CartSharingSetting"" s
                SET ""SharedWithId"" = (SELECT t.""SharedWithId"" FROM ""CartSharingSettingTarget"" t WHERE t.""CartSharingSettingId"" = s.""Id"" ORDER BY t.""CreatedDate"" LIMIT 1);
            ");

            migrationBuilder.DropTable(
                name: "CartSharingSettingTarget");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "CartSharingSetting");
        }
    }
}
