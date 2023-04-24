﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VirtoCommerce.CartModule.Data.PostgreSql.Migrations
{
    public partial class AddShipmentAndPaymentComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "CartShipment",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "CartPayment",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "CartShipment");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "CartPayment");
        }
    }
}
