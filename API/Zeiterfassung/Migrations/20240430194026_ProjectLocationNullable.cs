using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zeiterfassung.Migrations
{
    /// <inheritdoc />
    public partial class ProjectLocationNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSessions_Locations_LocationId",
                table: "WorkSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSessions_Projects_ProjectId",
                table: "WorkSessions");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "WorkSessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "WorkSessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSessions_Locations_LocationId",
                table: "WorkSessions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSessions_Projects_ProjectId",
                table: "WorkSessions",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSessions_Locations_LocationId",
                table: "WorkSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkSessions_Projects_ProjectId",
                table: "WorkSessions");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "WorkSessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "WorkSessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSessions_Locations_LocationId",
                table: "WorkSessions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSessions_Projects_ProjectId",
                table: "WorkSessions",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
