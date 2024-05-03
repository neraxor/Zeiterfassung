using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeiterfassung.Migrations
{
    /// <inheritdoc />
    public partial class WorkingHOursWeekly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkingHoursWeekly",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkingHoursWeekly",
                table: "Users");
        }
    }
}
