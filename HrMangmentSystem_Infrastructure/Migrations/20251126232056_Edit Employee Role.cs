using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrMangmentSystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditEmployeeRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeRoles",
                table: "EmployeeRoles");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "EmployeeRoles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EmployeeRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "EmployeeRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "EmployeeRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EmployeeRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "EmployeeRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeRoles",
                table: "EmployeeRoles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoles_EmployeeId_RoleId",
                table: "EmployeeRoles",
                columns: new[] { "EmployeeId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoles_TenantId",
                table: "EmployeeRoles",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeRoles_Tenants_TenantId",
                table: "EmployeeRoles",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeRoles_Tenants_TenantId",
                table: "EmployeeRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeRoles",
                table: "EmployeeRoles");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeRoles_EmployeeId_RoleId",
                table: "EmployeeRoles");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeRoles_TenantId",
                table: "EmployeeRoles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EmployeeRoles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EmployeeRoles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "EmployeeRoles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EmployeeRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EmployeeRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "EmployeeRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeRoles",
                table: "EmployeeRoles",
                columns: new[] { "EmployeeId", "RoleId" });
        }
    }
}
