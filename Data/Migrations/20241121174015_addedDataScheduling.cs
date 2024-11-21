using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class addedDataScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "AspNetUserID", "BirthDate", "EmployedSince", "FirstName", "LastName", "PlaceOfResidence", "StreetName", "StreetNumber" },
                values: new object[,]
                {
                    { 1, new DateOnly(1, 1, 1), new DateOnly(1, 1, 1), "John", "Doe", "New York", "5th Avenue", null },
                    { 2, new DateOnly(1, 1, 1), new DateOnly(1, 1, 1), "Jane", "Smith", "Los Angeles", "Sunset Boulevard", null }
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
                    { 1, "Stock Refill", (byte)0, "Vakkenvullen", 0, false },
                    { 2, "Customer Support", (byte)0, "Kassa", 0, false }
                });

            migrationBuilder.InsertData(
                table: "Week",
                columns: new[] { "ID", "WeekNumber", "Year" },
                values: new object[,]
                {
                    { 1, (byte)0, (short)0 },
                    { 2, (byte)0, (short)0 }
                });

            migrationBuilder.InsertData(
                table: "HolidayDay",
                columns: new[] { "Date", "Holiday_Name", "Impact" },
                values: new object[,]
                {
                    { new DateOnly(2024, 12, 25), "Christmas", (short)0 },
                    { new DateOnly(2025, 1, 1), "New Year", (short)0 }
                });

            migrationBuilder.InsertData(
                table: "PrognosisDay",
                columns: new[] { "PrognosisID", "Weekday", "VisitorEstimate" },
                values: new object[,]
                {
                    { 1, (byte)1, 0 },
                    { 1, (byte)2, 0 }
                });

            migrationBuilder.InsertData(
                table: "Shift",
                columns: new[] { "ID", "Department", "Employee", "EndTime", "ExternalEmployeeName", "StartTime", "WeekID", "Weekday" },
                values: new object[,]
                {
                    { 1, "Kassa", null, new TimeOnly(0, 0, 0), "Temporary Worker", new TimeOnly(0, 0, 0), 1, (byte)0 },
                    { 2, "Vakkenvullen", null, new TimeOnly(0, 0, 0), "Freelancer", new TimeOnly(0, 0, 0), 2, (byte)0 }
                });

            migrationBuilder.InsertData(
                table: "FillRequest",
                columns: new[] { "ID", "Absent_Description", "Accepted", "ShiftID", "SubstituteEmployeeID" },
                values: new object[,]
                {
                    { 1, "Illness", (byte)0, 1, null },
                    { 2, "Vacation", (byte)0, 2, null }
                });

            migrationBuilder.InsertData(
                table: "PrognosisDepartment",
                columns: new[] { "Department", "PrognosisID", "Weekday", "WorkHours" },
                values: new object[,]
                {
                    { "Kassa", 1, (byte)1, (short)0 },
                    { "Vakkenvullen", 1, (byte)2, (short)0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "AspNetUserID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "AspNetUserID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FillRequest",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FillRequest",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HolidayDay",
                keyColumns: new[] { "Date", "Holiday_Name" },
                keyValues: new object[] { new DateOnly(2024, 12, 25), "Christmas" });

            migrationBuilder.DeleteData(
                table: "HolidayDay",
                keyColumns: new[] { "Date", "Holiday_Name" },
                keyValues: new object[] { new DateOnly(2025, 1, 1), "New Year" });

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
                keyValues: new object[] { "Vakkenvullen", 1, (byte)2 });

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
                keyValues: new object[] { 1, (byte)2 });

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "ID",
                keyValue: 2);

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
