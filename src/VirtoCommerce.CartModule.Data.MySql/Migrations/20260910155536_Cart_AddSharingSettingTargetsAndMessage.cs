using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.MySql.Migrations
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
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CartSharingSettingTarget",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CartSharingSettingId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SharedWithId = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModifiedBy = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartSharingSettingTarget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartSharingSettingTarget_CartSharingSetting_CartSharingSetti~",
                        column: x => x.CartSharingSettingId,
                        principalTable: "CartSharingSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CartSharingSettingTarget_CartSharingSettingId",
                table: "CartSharingSettingTarget",
                column: "CartSharingSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_CartSharingSettingTarget_SharedWithId",
                table: "CartSharingSettingTarget",
                column: "SharedWithId");

            migrationBuilder.Sql(@"
                INSERT INTO `CartSharingSettingTarget` (`Id`, `CartSharingSettingId`, `SharedWithId`, `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy`)
                SELECT REPLACE(UUID(), '-', ''), `Id`, `SharedWithId`, `CreatedDate`, `ModifiedDate`, `CreatedBy`, `ModifiedBy`
                FROM `CartSharingSetting`
                WHERE `SharedWithId` IS NOT NULL;
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
                type: "varchar(128)",
                maxLength: 128,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(@"
                UPDATE `CartSharingSetting` s
                SET s.`SharedWithId` = (SELECT t.`SharedWithId` FROM `CartSharingSettingTarget` t WHERE t.`CartSharingSettingId` = s.`Id` ORDER BY t.`CreatedDate` LIMIT 1);
            ");

            migrationBuilder.DropTable(
                name: "CartSharingSettingTarget");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "CartSharingSetting");
        }
    }
}
