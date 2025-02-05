﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddCartConfigurationSectionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomText",
                table: "CartConfigurationItem",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "CartConfigurationItem",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomText",
                table: "CartConfigurationItem");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CartConfigurationItem");
        }
    }
}
