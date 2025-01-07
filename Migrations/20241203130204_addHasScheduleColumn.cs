using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class addHasScheduleColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "HasSchedule",
                table: "Week",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.UpdateData(
                table: "Week",
                keyColumn: "Id",
                keyValue: 1,
                column: "HasSchedule",
                value: (byte)0);

            migrationBuilder.UpdateData(
                table: "Week",
                keyColumn: "Id",
                keyValue: 2,
                column: "HasSchedule",
                value: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasSchedule",
                table: "Week");
        }
    }
}
