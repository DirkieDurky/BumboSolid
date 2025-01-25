﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BumboSolid.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewPull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

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
                    MaxShiftDuration = table.Column<int>(type: "int", nullable: true),
                    HolidaySurcharge = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLAEntry", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CLASurchargeEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Surcharge = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLASurchargeEntry", x => x.Id);
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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    LastName = table.Column<string>(type: "varchar(90)", unicode: false, maxLength: 90, nullable: false),
                    PlaceOfResidence = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    StreetName = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    StreetNumber = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployedSince = table.Column<DateOnly>(type: "date", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    WeekNumber = table.Column<byte>(type: "tinyint", nullable: false),
                    HasSchedule = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Week", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvailabilityRule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Employee = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Available = table.Column<byte>(type: "tinyint", nullable: false),
                    School = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilityRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailabilityRule_Employee",
                        column: x => x.Employee,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Capability",
                columns: table => new
                {
                    Employee = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capability", x => new { x.Employee, x.Department });
                    table.ForeignKey(
                        name: "FK_Capability_Department",
                        column: x => x.Department,
                        principalTable: "Department",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Capability_Employee",
                        column: x => x.Employee,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Absence",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeekId = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    AbsentDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Absence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Absence_Employee",
                        column: x => x.EmployeeID,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Absence_Week",
                        column: x => x.WeekId,
                        principalTable: "Week",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClockedHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeekID = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    Department = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Employee = table.Column<int>(type: "int", nullable: true),
                    ExternalEmployeeName = table.Column<string>(type: "varchar(135)", unicode: false, maxLength: 135, nullable: true),
                    IsBreak = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClockedHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClockedHours_Department",
                        column: x => x.Department,
                        principalTable: "Department",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_ClockedHours_Employee",
                        column: x => x.Employee,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClockedHours_Week",
                        column: x => x.WeekID,
                        principalTable: "Week",
                        principalColumn: "Id");
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
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Shift",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeekID = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    Department = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Employee = table.Column<int>(type: "int", nullable: true),
                    ExternalEmployeeName = table.Column<string>(type: "varchar(135)", unicode: false, maxLength: 135, nullable: true),
                    IsBreak = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shift", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shift_Department",
                        column: x => x.Department,
                        principalTable: "Department",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Shift_Employee",
                        column: x => x.Employee,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shift_Week",
                        column: x => x.WeekID,
                        principalTable: "Week",
                        principalColumn: "Id");
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShiftID = table.Column<int>(type: "int", nullable: false),
                    SubstituteEmployeeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FillRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FillRequest_Employee",
                        column: x => x.SubstituteEmployeeID,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FillRequest_Shift",
                        column: x => x.ShiftID,
                        principalTable: "Shift",
                        principalColumn: "Id");
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

            migrationBuilder.InsertData(
                table: "Week",
                columns: new[] { "Id", "HasSchedule", "WeekNumber", "Year" },
                values: new object[,]
                {
                    { 1, (byte)0, (byte)50, (short)2024 },
                    { 2, (byte)0, (byte)1, (short)2025 },
                    { 3, (byte)0, (byte)2, (short)2025 },
                    { 4, (byte)0, (byte)3, (short)2025 },
                    { 5, (byte)0, (byte)4, (short)2025 },
                    { 6, (byte)0, (byte)5, (short)2025 }
                });

            migrationBuilder.InsertData(
                table: "Norm",
                columns: new[] { "ID", "Activity", "AvgDailyPerformances", "Department", "Duration", "PerVisitor" },
                values: new object[,]
                {
                    { 1, "1 vak vullen", (byte)5, "Vakkenvullen", 300, false },
                    { 2, "Kassa", (byte)1, "Kassa", 120, true },
                    { 3, "Vers", (byte)8, "Vers", 45, false }
                });

            migrationBuilder.InsertData(
                table: "PrognosisDay",
                columns: new[] { "PrognosisID", "Weekday", "VisitorEstimate" },
                values: new object[,]
                {
                    { 1, (byte)0, 1000 },
                    { 1, (byte)1, 1000 },
                    { 1, (byte)2, 1000 },
                    { 1, (byte)3, 1000 },
                    { 1, (byte)4, 1000 },
                    { 1, (byte)5, 1000 },
                    { 1, (byte)6, 1000 }
                });

            migrationBuilder.InsertData(
                table: "PrognosisDepartment",
                columns: new[] { "Department", "PrognosisID", "Weekday", "WorkHours" },
                values: new object[,]
                {
                    { "Kassa", 1, (byte)0, (short)4000 },
                    { "Kassa", 1, (byte)1, (short)4000 },
                    { "Kassa", 1, (byte)2, (short)4000 },
                    { "Kassa", 1, (byte)3, (short)4000 },
                    { "Kassa", 1, (byte)4, (short)4000 },
                    { "Kassa", 1, (byte)5, (short)4000 },
                    { "Kassa", 1, (byte)6, (short)4000 },
                    { "Vakkenvullen", 1, (byte)0, (short)4000 },
                    { "Vakkenvullen", 1, (byte)1, (short)4000 },
                    { "Vakkenvullen", 1, (byte)2, (short)4000 },
                    { "Vakkenvullen", 1, (byte)3, (short)4000 },
                    { "Vakkenvullen", 1, (byte)4, (short)4000 },
                    { "Vakkenvullen", 1, (byte)5, (short)4000 },
                    { "Vakkenvullen", 1, (byte)6, (short)4000 },
                    { "Vers", 1, (byte)0, (short)4000 },
                    { "Vers", 1, (byte)1, (short)4000 },
                    { "Vers", 1, (byte)2, (short)4000 },
                    { "Vers", 1, (byte)3, (short)4000 },
                    { "Vers", 1, (byte)4, (short)4000 },
                    { "Vers", 1, (byte)5, (short)4000 },
                    { "Vers", 1, (byte)6, (short)4000 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Absence_EmployeeID",
                table: "Absence",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Absence_WeekID",
                table: "Absence",
                column: "WeekId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityRule_Employee",
                table: "AvailabilityRule",
                column: "Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Capability_Department",
                table: "Capability",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_ClockedHours_Department",
                table: "ClockedHours",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_ClockedHours_Employee",
                table: "ClockedHours",
                column: "Employee");

            migrationBuilder.CreateIndex(
                name: "IX_ClockedHours_WeekID",
                table: "ClockedHours",
                column: "WeekID");

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
                name: "IX_Shift_Employee",
                table: "Shift",
                column: "Employee");

            migrationBuilder.CreateIndex(
                name: "IX_Shift_WeekID",
                table: "Shift",
                column: "WeekID");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Absence");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AvailabilityRule");

            migrationBuilder.DropTable(
                name: "Capability");

            migrationBuilder.DropTable(
                name: "CLABreakEntry");

            migrationBuilder.DropTable(
                name: "CLASurchargeEntry");

            migrationBuilder.DropTable(
                name: "ClockedHours");

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
                name: "AspNetRoles");

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
                name: "Department");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Week");
        }
    }
}
