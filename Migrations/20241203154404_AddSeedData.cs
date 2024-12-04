using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 2, (byte)2 });

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Shift",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 2, (byte)2 });

            migrationBuilder.DeleteData(
                table: "Week",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Activity", "Duration" },
                values: new object[] { "1 vak vullen", 300 });

            migrationBuilder.UpdateData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "Activity", "AvgDailyPerformances", "Duration", "PerVisitor" },
                values: new object[] { "Kassa", (byte)1, 120, true });

            migrationBuilder.InsertData(
                table: "Norm",
                columns: new[] { "ID", "Activity", "AvgDailyPerformances", "Department", "Duration", "PerVisitor" },
                values: new object[] { 3, "Vers", (byte)8, "Vers", 45, false });

            migrationBuilder.UpdateData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)1 },
                column: "VisitorEstimate",
                value: 1000);

            migrationBuilder.InsertData(
                table: "PrognosisDay",
                columns: new[] { "PrognosisID", "Weekday", "VisitorEstimate" },
                values: new object[,]
                {
                    { 1, (byte)0, 1000 },
                    { 1, (byte)2, 1000 },
                    { 1, (byte)3, 1000 },
                    { 1, (byte)4, 1000 },
                    { 1, (byte)5, 1000 },
                    { 1, (byte)6, 1000 }
                });

            migrationBuilder.UpdateData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Kassa", 1, (byte)1 },
                column: "WorkHours",
                value: (short)4000);

            migrationBuilder.InsertData(
                table: "PrognosisDepartment",
                columns: new[] { "Department", "PrognosisID", "Weekday", "WorkHours" },
                values: new object[,]
                {
                    { "Vakkenvullen", 1, (byte)1, (short)4000 },
                    { "Vers", 1, (byte)1, (short)4000 }
                });

            migrationBuilder.UpdateData(
                table: "Week",
                keyColumn: "Id",
                keyValue: 1,
                column: "WeekNumber",
                value: (byte)50);

            migrationBuilder.InsertData(
                table: "PrognosisDepartment",
                columns: new[] { "Department", "PrognosisID", "Weekday", "WorkHours" },
                values: new object[,]
                {
                    { "Kassa", 1, (byte)0, (short)4000 },
                    { "Kassa", 1, (byte)2, (short)4000 },
                    { "Kassa", 1, (byte)3, (short)4000 },
                    { "Kassa", 1, (byte)4, (short)4000 },
                    { "Kassa", 1, (byte)5, (short)4000 },
                    { "Kassa", 1, (byte)6, (short)4000 },
                    { "Vakkenvullen", 1, (byte)0, (short)4000 },
                    { "Vakkenvullen", 1, (byte)2, (short)4000 },
                    { "Vakkenvullen", 1, (byte)3, (short)4000 },
                    { "Vakkenvullen", 1, (byte)4, (short)4000 },
                    { "Vakkenvullen", 1, (byte)5, (short)4000 },
                    { "Vakkenvullen", 1, (byte)6, (short)4000 },
                    { "Vers", 1, (byte)0, (short)4000 },
                    { "Vers", 1, (byte)2, (short)4000 },
                    { "Vers", 1, (byte)3, (short)4000 },
                    { "Vers", 1, (byte)4, (short)4000 },
                    { "Vers", 1, (byte)5, (short)4000 },
                    { "Vers", 1, (byte)6, (short)4000 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Kassa", 1, (byte)0 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Kassa", 1, (byte)2 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Kassa", 1, (byte)3 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Kassa", 1, (byte)4 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Kassa", 1, (byte)5 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Kassa", 1, (byte)6 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 1, (byte)0 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 1, (byte)1 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 1, (byte)2 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 1, (byte)3 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 1, (byte)4 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 1, (byte)5 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vakkenvullen", 1, (byte)6 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vers", 1, (byte)0 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vers", 1, (byte)1 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vers", 1, (byte)2 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vers", 1, (byte)3 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vers", 1, (byte)4 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vers", 1, (byte)5 });

            migrationBuilder.DeleteData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Vers", 1, (byte)6 });

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)0 });

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)2 });

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)3 });

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)4 });

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)5 });

            migrationBuilder.DeleteData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)6 });

            migrationBuilder.UpdateData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Activity", "Duration" },
                values: new object[] { "Stocking", 60 });

            migrationBuilder.UpdateData(
                table: "Norm",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "Activity", "AvgDailyPerformances", "Duration", "PerVisitor" },
                values: new object[] { "Cashier", (byte)8, 45, false });

            migrationBuilder.UpdateData(
                table: "PrognosisDay",
                keyColumns: new[] { "PrognosisID", "Weekday" },
                keyValues: new object[] { 1, (byte)1 },
                column: "VisitorEstimate",
                value: 0);

            migrationBuilder.UpdateData(
                table: "PrognosisDepartment",
                keyColumns: new[] { "Department", "PrognosisID", "Weekday" },
                keyValues: new object[] { "Kassa", 1, (byte)1 },
                column: "WorkHours",
                value: (short)0);

            migrationBuilder.UpdateData(
                table: "Week",
                keyColumn: "Id",
                keyValue: 1,
                column: "WeekNumber",
                value: (byte)1);

            migrationBuilder.InsertData(
                table: "Week",
                columns: new[] { "Id", "HasSchedule", "WeekNumber", "Year" },
                values: new object[] { 2, (byte)0, (byte)2, (short)2024 });

            migrationBuilder.InsertData(
                table: "PrognosisDay",
                columns: new[] { "PrognosisID", "Weekday", "VisitorEstimate" },
                values: new object[] { 2, (byte)2, 0 });

            migrationBuilder.InsertData(
                table: "Shift",
                columns: new[] { "Id", "Department", "Employee", "EndTime", "ExternalEmployeeName", "IsBreak", "StartTime", "WeekID", "Weekday" },
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

            migrationBuilder.InsertData(
                table: "PrognosisDepartment",
                columns: new[] { "Department", "PrognosisID", "Weekday", "WorkHours" },
                values: new object[] { "Vakkenvullen", 2, (byte)2, (short)0 });
        }
    }
}
