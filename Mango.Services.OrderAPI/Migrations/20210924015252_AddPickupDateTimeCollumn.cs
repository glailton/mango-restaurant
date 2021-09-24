using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mango.OrderAPI.Migrations
{
    public partial class AddPickupDateTimeCollumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PickupDateTime",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupDateTime",
                table: "OrderHeaders");
        }
    }
}
