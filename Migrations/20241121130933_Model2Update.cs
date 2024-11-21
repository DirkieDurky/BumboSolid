﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class Model2Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLAEntry",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgeStart = table.Column<int>(type: "int", nullable: true),
                    AgeEnd = table.Column<int>(type: "int", nullable: true),
                    MaxWorkDurationPerDay = table.Column<int>(type: "int", nullable: true),
                    MaxWorkDaysPerWeek = table.Column<int>(type: "int", nullable: true),
                    MaxWorkDurationPerWeek = table.Column<int>(type: "int", nullable: true),
                    MaxWorkDurationPerHolidayWeek = table.Column<int>(type: "int", nullable: true),
                    EarliestWorkTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    LatestWorkTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    MaxAvgWeeklyWorkDurationOverFourWeeks = table.Column<int>(type: "int", nullable: true),
                    MaxShiftDuration = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLAEntry", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    AspNetUserID = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    LastName = table.Column<string>(type: "varchar(90)", unicode: false, maxLength: 90, nullable: false),
                    PlaceOfResidence = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    StreetName = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    StreetNumber = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployedSince = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.AspNetUserID);
                });

            migrationBuilder.CreateTable(
                name: "FactorType",
                columns: table => new
                {
                    Type = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactorType", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "Holiday",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holiday", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Weather",
                columns: table => new
                {
                    ID = table.Column<byte>(type: "tinyint", nullable: false),
                    Impact = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weather", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Week",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    WeekNumber = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Week", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CLABreakEntry",
                columns: table => new
                {
                    CLAEntryId = table.Column<int>(type: "int", nullable: false),
                    WorkDuration = table.Column<int>(type: "int", nullable: false),
                    MinBreakDuration = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLABreakEntry", x => new { x.CLAEntryId, x.WorkDuration });
                    table.ForeignKey(
                        name: "FK_CLABreakEntry_CLAEntry",
                        column: x => x.CLAEntryId,
                        principalTable: "CLAEntry",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Norm",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Activity = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    AvgDailyPerformances = table.Column<byte>(type: "tinyint", nullable: false),
                    PerVisitor = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Norm", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Norm_Department",
                        column: x => x.Department,
                        principalTable: "Department",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "AvailabilityDay",
                columns: table => new
                {
                    Employee = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    LessonHours = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilityDay", x => new { x.Employee, x.Date });
                    table.ForeignKey(
                        name: "FK_AvailabilityDay_Employee",
                        column: x => x.Employee,
                        principalTable: "Employee",
                        principalColumn: "AspNetUserID");
                });

            migrationBuilder.CreateTable(
                name: "HolidayDay",
                columns: table => new
                {
                    Holiday_Name = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Impact = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayDay", x => new { x.Holiday_Name, x.Date });
                    table.ForeignKey(
                        name: "FK_HolidayDay_Holiday",
                        column: x => x.Holiday_Name,
                        principalTable: "Holiday",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "PrognosisDay",
                columns: table => new
                {
                    PrognosisID = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    VisitorEstimate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrognosisDay", x => new { x.PrognosisID, x.Weekday });
                    table.ForeignKey(
                        name: "FK_PrognosisDay_Week",
                        column: x => x.PrognosisID,
                        principalTable: "Week",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    WeekID = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    Department = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Employee = table.Column<int>(type: "int", nullable: true),
                    ExternalEmployeeName = table.Column<string>(type: "varchar(135)", unicode: false, maxLength: 135, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shift", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Shift_Department",
                        column: x => x.Department,
                        principalTable: "Department",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Shift_Week",
                        column: x => x.WeekID,
                        principalTable: "Week",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "AvailabilityRule",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Employee = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Available = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilityRule", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AvailabilityRule_AvailabilityDay",
                        columns: x => new { x.Employee, x.Date },
                        principalTable: "AvailabilityDay",
                        principalColumns: new[] { "Employee", "Date" });
                });

            migrationBuilder.CreateTable(
                name: "Factor",
                columns: table => new
                {
                    PrognosisID = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    WeatherID = table.Column<byte>(type: "tinyint", nullable: true),
                    Impact = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factor", x => new { x.PrognosisID, x.Type, x.Weekday });
                    table.ForeignKey(
                        name: "FK_Factor_FactorType",
                        column: x => x.Type,
                        principalTable: "FactorType",
                        principalColumn: "Type");
                    table.ForeignKey(
                        name: "FK_Factor_PrognosisDay",
                        columns: x => new { x.PrognosisID, x.Weekday },
                        principalTable: "PrognosisDay",
                        principalColumns: new[] { "PrognosisID", "Weekday" });
                    table.ForeignKey(
                        name: "FK_Factor_Weather",
                        column: x => x.WeatherID,
                        principalTable: "Weather",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PrognosisDepartment",
                columns: table => new
                {
                    PrognosisID = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    WorkHours = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrognosisDepartment", x => new { x.PrognosisID, x.Department, x.Weekday });
                    table.ForeignKey(
                        name: "FK_PrognosisDepartment_Department",
                        column: x => x.Department,
                        principalTable: "Department",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_PrognosisDepartment_PrognosisDay",
                        columns: x => new { x.PrognosisID, x.Weekday },
                        principalTable: "PrognosisDay",
                        principalColumns: new[] { "PrognosisID", "Weekday" });
                });

            migrationBuilder.CreateTable(
                name: "FillRequest",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    ShiftID = table.Column<int>(type: "int", nullable: false),
                    SubstituteEmployeeID = table.Column<int>(type: "int", nullable: true),
                    Absent_Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Accepted = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FillRequest", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FillRequest_Employee",
                        column: x => x.SubstituteEmployeeID,
                        principalTable: "Employee",
                        principalColumn: "AspNetUserID");
                    table.ForeignKey(
                        name: "FK_FillRequest_Shift",
                        column: x => x.ShiftID,
                        principalTable: "Shift",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "Department",
                column: "Name",
                values: new object[]
                {
                    "Kassa",
                    "Vakkenvullen",
                    "Vers"
                });

            migrationBuilder.InsertData(
                table: "FactorType",
                column: "Type",
                values: new object[]
                {
                    "Feestdagen",
                    "Overig",
                    "Weer"
                });

            migrationBuilder.InsertData(
                table: "Weather",
                columns: new[] { "ID", "Impact" },
                values: new object[,]
                {
                    { (byte)0, (short)75 },
                    { (byte)1, (short)50 },
                    { (byte)2, (short)25 },
                    { (byte)3, (short)0 },
                    { (byte)4, (short)-25 },
                    { (byte)5, (short)-50 },
                    { (byte)6, (short)-75 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityRule_Employee_Date",
                table: "AvailabilityRule",
                columns: new[] { "Employee", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Factor_PrognosisID_Weekday",
                table: "Factor",
                columns: new[] { "PrognosisID", "Weekday" });

            migrationBuilder.CreateIndex(
                name: "IX_Factor_Type",
                table: "Factor",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Factor_WeatherID",
                table: "Factor",
                column: "WeatherID");

            migrationBuilder.CreateIndex(
                name: "IX_FillRequest_ShiftID",
                table: "FillRequest",
                column: "ShiftID");

            migrationBuilder.CreateIndex(
                name: "IX_FillRequest_SubstituteEmployeeID",
                table: "FillRequest",
                column: "SubstituteEmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Norm_Department",
                table: "Norm",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_PrognosisDepartment_Department",
                table: "PrognosisDepartment",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_PrognosisDepartment_PrognosisID_Weekday",
                table: "PrognosisDepartment",
                columns: new[] { "PrognosisID", "Weekday" });

            migrationBuilder.CreateIndex(
                name: "IX_Shift_Department",
                table: "Shift",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_WeekID",
                table: "Shift",
                column: "WeekID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailabilityRule");

            migrationBuilder.DropTable(
                name: "CLABreakEntry");

            migrationBuilder.DropTable(
                name: "Factor");

            migrationBuilder.DropTable(
                name: "FillRequest");

            migrationBuilder.DropTable(
                name: "HolidayDay");

            migrationBuilder.DropTable(
                name: "Norm");

            migrationBuilder.DropTable(
                name: "PrognosisDepartment");

            migrationBuilder.DropTable(
                name: "AvailabilityDay");

            migrationBuilder.DropTable(
                name: "CLAEntry");

            migrationBuilder.DropTable(
                name: "FactorType");

            migrationBuilder.DropTable(
                name: "Weather");

            migrationBuilder.DropTable(
                name: "Shift");

            migrationBuilder.DropTable(
                name: "Holiday");

            migrationBuilder.DropTable(
                name: "PrognosisDay");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Week");
        }
    }
}
