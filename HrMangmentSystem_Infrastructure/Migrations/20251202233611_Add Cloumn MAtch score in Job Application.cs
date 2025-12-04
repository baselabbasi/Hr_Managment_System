using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrMangmentSystem_Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCloumnMAtchscoreinJobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MatchScore",
                table: "JobApplications",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchScore",
                table: "JobApplications");
        }
    }
}
