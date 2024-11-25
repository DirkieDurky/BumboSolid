using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class AddedDummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CLAEntry",
                columns: new[] { "ID", "AgeEnd", "AgeStart", "EarliestWorkTime", "LatestWorkTime", "MaxAvgWeeklyWorkDurationOverFourWeeks", "MaxShiftDuration", "MaxWorkDaysPerWeek", "MaxWorkDurationPerDay", "MaxWorkDurationPerHolidayWeek", "MaxWorkDurationPerWeek" },
                values: new object[,]
                {
                    { 1, null, null, null, null, 38, 8, 5, 8, 35, 40 },
                    { 2, null, null, null, null, 33, 7, 5, 7, 30, 35 }
                });

            migrationBuilder.InsertData(
                table: "Holiday",
                column: "Name",
                values: new object[]
                {
                    "Christmas",
                    "New Year"
                });

            migrationBuilder.InsertData(
                table: "Norm",
                columns: new[] { "ID", "Activity", "AvgDailyPerformances", "Department", "Duration", "PerVisitor" },
                values: new object[,]
                {
                    { 1, "Stocking", (byte)5, "Vakkenvullen", 60, false },
                    { 2, "Cashier", (byte)8, "Kassa", 45, false }
                });

            migrationBuilder.InsertData(
                table: "Week",
                columns: new[] { "ID", "WeekNumber", "Year" },
                values: new object[,]
                {
                    { 1, (byte)1, (short)2024 },
                    { 2, (byte)2, (short)2024 }
                });

            migrationBuilder.InsertData(
                table: "CLABreakEntry",
                columns: new[] { "CLAEntryId", "WorkDuration", "MinBreakDuration" },
                values: new object[,]
                {
                    { 1, 4, 30 },
                    { 2, 5, 45 }
                });

            migrationBuilder.InsertData(
                table: "HolidayDay",
                columns: new[] { "Date", "Holiday_Name", "Impact" },
                values: new object[,]
                {
                    { new DateOnly(2023, 12, 25), "Christmas", (short)0 },
                    { new DateOnly(2023, 1, 1), "New Year", (short)0 }
                });

            migrationBuilder.InsertData(
                table: "PrognosisDay",
                columns: new[] { "PrognosisID", "Weekday", "VisitorEstimate" },
                values: new object[,]
                {
                    { 1, (byte)1, 0 },
                    { 2, (byte)2, 0 }
                });

            migrationBuilder.InsertData(
                table: "Shift",
                columns: new[] { "ID", "Department", "Employee", "EndTime", "ExternalEmployeeName", "IsBreak", "StartTime", "WeekID", "Weekday" },
                values: new object[,]
                {
                    { 1, "Kassa", null, new TimeOnly(0, 0, 0), "John Doe", (byte)0, new TimeOnly(0, 0, 0), 1, (byte)0 },
                    { 2, "Vakkenvullen", null, new TimeOnly(0, 0, 0), "Jane Smith", (byte)0, new TimeOnly(0, 0, 0), 2, (byte)0 }
                });

            migrationBuilder.InsertData(
                table: "PrognosisDepartment",
                columns: new[] { "Department", "PrognosisID", "Weekday", "WorkHours" },
                values: new object[,]
                {
                    { "Kassa", 1, (byte)1, (short)0 },
                    { "Vakkenvullen", 2, (byte)2, (short)0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CLABreakEntry",
                keyColumns: new[] { "CLAEntryId", "WorkDuration" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "CLABreakEntry",
                keyColumns: new[] { "CLAEntryId", "WorkDuration" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "HolidayDay",
                keyColumns: new[] { "Date", "Holiday_Name" },
                keyValues: new object[] { new DateOnly(2023, 12, 25), "Christmas" });

            migrationBuilder.DeleteData(
                table: "HolidayDay",
                keyColumns: new[] { "Date", "Holiday_Name" },
                keyValues: new object[] { new DateOnly(2023, 1, 1), "New Year" });

            migrationBuilder.DeleteData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Kassa", 1, (byte)1 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 2, (byte)2 });

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CLAEntry",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CLAEntry",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Holiday",
                keyColumn: "Name",
                keyValue: "Christmas");

            migrationBuilder.DeleteData(
                table: "Holiday",
                keyColumn: "Name",
                keyValue: "New Year");

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)1 });

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 2, (byte)2 });

            migrationBuilder.DeleteData(
                table: "Week",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Week",
                keyColumn: "ID",
                keyValue: 2);
        }
    }
}
