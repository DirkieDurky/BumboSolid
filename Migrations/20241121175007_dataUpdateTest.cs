using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class dataUpdateTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HolidayDay",
                keyColumns: new[] { "Date", "Holiday_Name" },
                keyValues: new object[] { new DateOnly(2024, 12, 25), "Christmas" });

            migrationBuilder.DeleteData(
                table: "HolidayDay",
                keyColumns: new[] { "Date", "Holiday_Name" },
                keyValues: new object[] { new DateOnly(2025, 1, 1), "New Year" });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 1, (byte)2 });

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)2 });

            migrationBuilder.InsertData(
                table: "AvailabilityDay",
                columns: new[] { "Date", "Employee", "LessonHours" },
                values: new object[,]
                {
                    { new DateOnly(2023, 1, 1), 1, null },
                    { new DateOnly(2023, 1, 2), 2, null }
                });

            migrationBuilder.InsertData(
                table: "CLAEntry",
                columns: new[] { "ID", "AgeEnd", "AgeStart", "EarliestWorkTime", "LatestWorkTime", "MaxAvgWeeklyWorkDurationOverFourWeeks", "MaxShiftDuration", "MaxWorkDaysPerWeek", "MaxWorkDurationPerDay", "MaxWorkDurationPerHolidayWeek", "MaxWorkDurationPerWeek" },
                values: new object[,]
                {
                    { 1, null, null, null, null, 38, 8, 5, 8, 35, 40 },
                    { 2, null, null, null, null, 33, 7, 5, 7, 30, 35 }
                });

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "AspNetUserID",
                keyValue: 1,
                columns: new[] { "PlaceOfResidence", "StreetName" },
                values: new object[] { "City", "Street 1" });

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "AspNetUserID",
                keyValue: 2,
                columns: new[] { "PlaceOfResidence", "StreetName" },
                values: new object[] { "Town", "Street 2" });

            migrationBuilder.UpdateData(
                table: "FillRequest",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Absent_Description", "SubstituteEmployeeID" },
                values: new object[] { "Sick", 2 });

            migrationBuilder.UpdateData(
                table: "FillRequest",
                keyColumn: "ID",
                keyValue: 2,
                column: "SubstituteEmployeeID",
                value: 1);

            migrationBuilder.InsertData(
                table: "HolidayDay",
                columns: new[] { "Date", "Holiday_Name", "Impact" },
                values: new object[,]
                {
                    { new DateOnly(2023, 12, 25), "Christmas", (short)0 },
                    { new DateOnly(2023, 1, 1), "New Year", (short)0 }
                });

            migrationBuilder.UpdateData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Activity", "AvgDailyPerformances", "Duration" },
                values: new object[] { "Stocking", (byte)5, 60 });

            migrationBuilder.UpdateData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "Activity", "AvgDailyPerformances", "Duration" },
                values: new object[] { "Cashier", (byte)8, 45 });

            migrationBuilder.InsertData(
                table: "PrognosisDay",
                columns: new[] { "PrognosisID", "Weekday", "VisitorEstimate" },
                values: new object[] { 2, (byte)2, 0 });

            migrationBuilder.UpdateData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 1,
                column: "ExternalEmployeeName",
                value: "John Doe");

            migrationBuilder.UpdateData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 2,
                column: "ExternalEmployeeName",
                value: "Jane Smith");

            migrationBuilder.InsertData(
                table: "AvailabilityRule",
                columns: new[] { "ID", "Available", "Date", "Employee", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { 1, (byte)0, new DateOnly(2023, 1, 1), 1, new TimeOnly(0, 0, 0), new TimeOnly(0, 0, 0) },
                    { 2, (byte)0, new DateOnly(2023, 1, 2), 2, new TimeOnly(0, 0, 0), new TimeOnly(0, 0, 0) }
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
                table: "PrognosisDepartment",
                columns: new[] { "Department", "PrognosisID", "Weekday", "WorkHours" },
                values: new object[] { "Vakkenvullen", 2, (byte)2, (short)0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AvailabilityRule",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AvailabilityRule",
                keyColumn: "ID",
                keyValue: 2);

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
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 2, (byte)2 });

            migrationBuilder.DeleteData(
                table: "AvailabilityDay",
                keyColumns: new[] { "Date", "Employee" },
                keyValues: new object[] { new DateOnly(2023, 1, 1), 1 });

            migrationBuilder.DeleteData(
                table: "AvailabilityDay",
                keyColumns: new[] { "Date", "Employee" },
                keyValues: new object[] { new DateOnly(2023, 1, 2), 2 });

            migrationBuilder.DeleteData(
                table: "CLAEntry",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CLAEntry",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 2, (byte)2 });

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "AspNetUserID",
                keyValue: 1,
                columns: new[] { "PlaceOfResidence", "StreetName" },
                values: new object[] { "New York", "5th Avenue" });

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "AspNetUserID",
                keyValue: 2,
                columns: new[] { "PlaceOfResidence", "StreetName" },
                values: new object[] { "Los Angeles", "Sunset Boulevard" });

            migrationBuilder.UpdateData(
                table: "FillRequest",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Absent_Description", "SubstituteEmployeeID" },
                values: new object[] { "Illness", null });

            migrationBuilder.UpdateData(
                table: "FillRequest",
                keyColumn: "ID",
                keyValue: 2,
                column: "SubstituteEmployeeID",
                value: null);

            migrationBuilder.InsertData(
                table: "HolidayDay",
                columns: new[] { "Date", "Holiday_Name", "Impact" },
                values: new object[,]
                {
                    { new DateOnly(2024, 12, 25), "Christmas", (short)0 },
                    { new DateOnly(2025, 1, 1), "New Year", (short)0 }
                });

            migrationBuilder.UpdateData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Activity", "AvgDailyPerformances", "Duration" },
                values: new object[] { "Stock Refill", (byte)0, 0 });

            migrationBuilder.UpdateData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "Activity", "AvgDailyPerformances", "Duration" },
                values: new object[] { "Customer Support", (byte)0, 0 });

            migrationBuilder.InsertData(
                table: "PrognosisDay",
                columns: new[] { "PrognosisID", "Weekday", "VisitorEstimate" },
                values: new object[] { 1, (byte)2, 0 });

            migrationBuilder.UpdateData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 1,
                column: "ExternalEmployeeName",
                value: "Temporary Worker");

            migrationBuilder.UpdateData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 2,
                column: "ExternalEmployeeName",
                value: "Freelancer");

            migrationBuilder.InsertData(
                table: "PrognosisDepartment",
                columns: new[] { "Department", "PrognosisID", "Weekday", "WorkHours" },
                values: new object[] { "Vakkenvullen", 1, (byte)2, (short)0 });
        }
    }
}
