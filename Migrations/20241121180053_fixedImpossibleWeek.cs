using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class fixedImpossibleWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Week",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "WeekNumber", "Year" },
                values: new object[] { (byte)1, (short)2024 });

            migrationBuilder.UpdateData(
                table: "Week",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "WeekNumber", "Year" },
                values: new object[] { (byte)2, (short)2024 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Week",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "WeekNumber", "Year" },
                values: new object[] { (byte)0, (short)0 });

            migrationBuilder.UpdateData(
                table: "Week",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "WeekNumber", "Year" },
                values: new object[] { (byte)0, (short)0 });
        }
    }
}
