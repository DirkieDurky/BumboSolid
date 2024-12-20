using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class ClockingHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClockedHours");
        }
    }
}
