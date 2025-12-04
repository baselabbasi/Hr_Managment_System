using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrMangmentSystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveJobPositionCreatedByEmployeeFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPositions_Employees_CreatedByEmployeeId",
                table: "JobPositions");

            migrationBuilder.DropIndex(
                name: "IX_JobPositions_CreatedByEmployeeId",
                table: "JobPositions");

            migrationBuilder.DropColumn(
                name: "CreatedByEmployeeId",
                table: "JobPositions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByEmployeeId",
                table: "JobPositions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_JobPositions_CreatedByEmployeeId",
                table: "JobPositions",
                column: "CreatedByEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPositions_Employees_CreatedByEmployeeId",
                table: "JobPositions",
                column: "CreatedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
