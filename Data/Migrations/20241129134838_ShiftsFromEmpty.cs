using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class ShiftsFromEmpty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "Shift",
                columns: new[] { "ID", "Department", "Employee", "EndTime", "ExternalEmployeeName", "IsBreak", "StartTime", "WeekID", "Weekday" },
                values: new object[,]
                {
                    { 3, "Kassa", null, new TimeOnly(17, 5, 0), "Alice Johnson", (byte)0, new TimeOnly(9, 0, 0), 2, (byte)2 },
                    { 4, "Vakkenvullen", null, new TimeOnly(18, 5, 0), "Bob Brown", (byte)0, new TimeOnly(10, 55, 0), 2, (byte)5 },
                    { 5, "Kassa", null, new TimeOnly(16, 5, 0), "Charlie Davis", (byte)0, new TimeOnly(8, 0, 0), 2, (byte)1 },
                    { 6, "Vakkenvullen", null, new TimeOnly(19, 0, 0), "Diana Evans", (byte)0, new TimeOnly(11, 0, 0), 2, (byte)3 },
                    { 7, "Kassa", null, new TimeOnly(15, 0, 0), "Ethan Foster", (byte)0, new TimeOnly(7, 0, 0), 2, (byte)0 },
                    { 8, "Vakkenvullen", null, new TimeOnly(20, 0, 0), "Fiona Green", (byte)0, new TimeOnly(12, 0, 0), 2, (byte)4 },
                    { 9, "Kassa", null, new TimeOnly(21, 5, 0), "George Harris", (byte)0, new TimeOnly(13, 0, 0), 2, (byte)6 },
                    { 10, "Vakkenvullen", null, new TimeOnly(22, 30, 0), "Hannah Lee", (byte)0, new TimeOnly(14, 0, 0), 2, (byte)2 },
                    { 11, "Kassa", null, new TimeOnly(23, 0, 0), "Ian Miller", (byte)0, new TimeOnly(15, 0, 0), 2, (byte)5 },
                    { 12, "Vakkenvullen", null, new TimeOnly(0, 0, 0), "Julia Nelson", (byte)0, new TimeOnly(16, 0, 0), 2, (byte)1 },
                    { 13, "Kassa", null, new TimeOnly(1, 0, 0), "Kevin Owens", (byte)0, new TimeOnly(17, 0, 0), 2, (byte)3 },
                    { 14, "Vakkenvullen", null, new TimeOnly(2, 0, 0), "Laura Perez", (byte)0, new TimeOnly(18, 0, 0), 2, (byte)0 },
                    { 15, "Kassa", null, new TimeOnly(3, 0, 0), "Michael Quinn", (byte)0, new TimeOnly(10, 0, 0), 2, (byte)4 },
                    { 16, "Kassa", null, new TimeOnly(5, 30, 0), "Nina Roberts", (byte)0, new TimeOnly(20, 0, 0), 2, (byte)5 },
                    { 17, "Vakkenvullen", null, new TimeOnly(5, 20, 0), "Oscar Scott", (byte)0, new TimeOnly(20, 0, 0), 2, (byte)5 },
                    { 18, "Vakkenvullen", null, new TimeOnly(5, 10, 0), "Paula Turner", (byte)0, new TimeOnly(20, 0, 0), 2, (byte)5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 18);

            migrationBuilder.InsertData(
                table: "Shift",
                columns: new[] { "ID", "Department", "Employee", "EndTime", "ExternalEmployeeName", "IsBreak", "StartTime", "WeekID", "Weekday" },
                values: new object[,]
                {
                    { 1, "Kassa", null, new TimeOnly(0, 0, 0), "John Doe", (byte)0, new TimeOnly(0, 0, 0), 1, (byte)0 },
                    { 2, "Vakkenvullen", null, new TimeOnly(0, 0, 0), "Jane Smith", (byte)0, new TimeOnly(0, 0, 0), 2, (byte)0 }
                });
        }
    }
}
