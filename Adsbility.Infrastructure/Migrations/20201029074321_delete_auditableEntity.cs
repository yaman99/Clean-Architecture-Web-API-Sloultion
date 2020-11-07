using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Adsbility.Infrastructure.Migrations
{
    public partial class delete_auditableEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Test");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Test");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Test",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Test",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Test",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Test",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
