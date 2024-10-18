using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Data.Migrations
{
    /// <inheritdoc />
    public partial class BumboMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Function",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Function", x => x.Name);
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
                name: "Prognosis",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    Week = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prognosis", x => x.ID);
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
                name: "Norm",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false),
                    Activity = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Function = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    AvgDailyPerformances = table.Column<byte>(type: "tinyint", nullable: false),
                    PerVisitor = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Norm", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Norm_Function",
                        column: x => x.Function,
                        principalTable: "Function",
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
                name: "PrognosisDay",
                columns: table => new
                {
                    Prognosis_ID = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    VisitorEstimate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrognosisDay", x => new { x.Prognosis_ID, x.Weekday });
                    table.ForeignKey(
                        name: "FK_PrognosisDay_Prognosis",
                        column: x => x.Prognosis_ID,
                        principalTable: "Prognosis",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Factor",
                columns: table => new
                {
                    Prognosis_ID = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    Weather_ID = table.Column<byte>(type: "tinyint", nullable: true),
                    Impact = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factor", x => new { x.Prognosis_ID, x.Type, x.Weekday });
                    table.ForeignKey(
                        name: "FK_Factor_FactorType",
                        column: x => x.Type,
                        principalTable: "FactorType",
                        principalColumn: "Type");
                    table.ForeignKey(
                        name: "FK_Factor_PrognosisDay",
                        columns: x => new { x.Prognosis_ID, x.Weekday },
                        principalTable: "PrognosisDay",
                        principalColumns: new[] { "Prognosis_ID", "Weekday" });
                    table.ForeignKey(
                        name: "FK_Factor_Weather",
                        column: x => x.Weather_ID,
                        principalTable: "Weather",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "PrognosisFunction",
                columns: table => new
                {
                    Prognosis_ID = table.Column<int>(type: "int", nullable: false),
                    Function = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: false),
                    Staff = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    WorkHours = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrognosisFunction", x => new { x.Prognosis_ID, x.Function, x.Weekday });
                    table.ForeignKey(
                        name: "FK_PrognosisFunction_Function",
                        column: x => x.Function,
                        principalTable: "Function",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_PrognosisFunction_PrognosisDay",
                        columns: x => new { x.Prognosis_ID, x.Weekday },
                        principalTable: "PrognosisDay",
                        principalColumns: new[] { "Prognosis_ID", "Weekday" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Factor_Prognosis_ID_Weekday",
                table: "Factor",
                columns: new[] { "Prognosis_ID", "Weekday" });

            migrationBuilder.CreateIndex(
                name: "IX_Factor_Type",
                table: "Factor",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Factor_Weather_ID",
                table: "Factor",
                column: "Weather_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Norm_Function",
                table: "Norm",
                column: "Function");

            migrationBuilder.CreateIndex(
                name: "IX_PrognosisFunction_Function",
                table: "PrognosisFunction",
                column: "Function");

            migrationBuilder.CreateIndex(
                name: "IX_PrognosisFunction_Prognosis_ID_Weekday",
                table: "PrognosisFunction",
                columns: new[] { "Prognosis_ID", "Weekday" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Factor");

            migrationBuilder.DropTable(
                name: "HolidayDay");

            migrationBuilder.DropTable(
                name: "Norm");

            migrationBuilder.DropTable(
                name: "PrognosisFunction");

            migrationBuilder.DropTable(
                name: "FactorType");

            migrationBuilder.DropTable(
                name: "Weather");

            migrationBuilder.DropTable(
                name: "Holiday");

            migrationBuilder.DropTable(
                name: "Function");

            migrationBuilder.DropTable(
                name: "PrognosisDay");

            migrationBuilder.DropTable(
                name: "Prognosis");
        }
    }
}
