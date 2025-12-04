using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrMangmentSystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditCloumninJobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Employees_ReviewedByEmployeeIdId",
                table: "JobApplications");

            migrationBuilder.RenameColumn(
                name: "ReviewedByEmployeeIdId",
                table: "JobApplications",
                newName: "ReviewedByEmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_ReviewedByEmployeeIdId",
                table: "JobApplications",
                newName: "IX_JobApplications_ReviewedByEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Employees_ReviewedByEmployeeId",
                table: "JobApplications",
                column: "ReviewedByEmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Employees_ReviewedByEmployeeId",
                table: "JobApplications");

            migrationBuilder.RenameColumn(
                name: "ReviewedByEmployeeId",
                table: "JobApplications",
                newName: "ReviewedByEmployeeIdId");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_ReviewedByEmployeeId",
                table: "JobApplications",
                newName: "IX_JobApplications_ReviewedByEmployeeIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Employees_ReviewedByEmployeeIdId",
                table: "JobApplications",
                column: "ReviewedByEmployeeIdId",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
