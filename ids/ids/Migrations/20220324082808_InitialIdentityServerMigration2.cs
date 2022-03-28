using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ids.Migrations
{
    public partial class InitialIdentityServerMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "PersistedGrants",
                type: "nvarchar(max)",
                maxLength: 50000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50000)",
                oldMaxLength: 50000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Expiration",
                table: "DeviceCodes",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "DeviceCodes",
                type: "nvarchar(max)",
                maxLength: 50000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50000)",
                oldMaxLength: 50000);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "PersistedGrants",
                type: "nvarchar(50000)",
                maxLength: 50000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 50000);

            migrationBuilder.AlterColumn<string>(
                name: "Expiration",
                table: "DeviceCodes",
                type: "nvarchar(200)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "DeviceCodes",
                type: "nvarchar(50000)",
                maxLength: 50000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 50000);
        }
    }
}
