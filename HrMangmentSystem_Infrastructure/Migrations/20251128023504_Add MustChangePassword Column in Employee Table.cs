using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrMangmentSystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMustChangePasswordColumninEmployeeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChangeAt",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "LastPasswordChangeAt", "MustChangePassword" },
                values: new object[] { null, false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPasswordChangeAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Employees");
        }
    }
}
