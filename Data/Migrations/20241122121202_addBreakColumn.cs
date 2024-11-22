using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class addBreakColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilityRule_AvailabilityDay",
                table: "AvailabilityRule");

            migrationBuilder.DropTable(
                name: "AvailabilityDay");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AvailabilityRule",
                table: "AvailabilityRule");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilityRule_Employee_Date",
                table: "AvailabilityRule");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "AvailabilityRule");

            migrationBuilder.AddColumn<byte>(
                name: "IsBreak",
                table: "Shift",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "School",
                table: "AvailabilityRule",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AvailabilityRule",
                table: "AvailabilityRule",
                columns: new[] { "Employee", "Date" });

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
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Capability_Employee",
                        column: x => x.Employee,
                        principalTable: "Employee",
                        principalColumn: "AspNetUserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Capability_Department",
                table: "Capability",
                column: "Department");

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilityRule_Employee",
                table: "AvailabilityRule",
                column: "Employee",
                principalTable: "Employee",
                principalColumn: "AspNetUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilityRule_Employee",
                table: "AvailabilityRule");

            migrationBuilder.DropTable(
                name: "Capability");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AvailabilityRule",
                table: "AvailabilityRule");

            migrationBuilder.DropColumn(
                name: "IsBreak",
                table: "Shift");

            migrationBuilder.DropColumn(
                name: "School",
                table: "AvailabilityRule");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "AvailabilityRule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AvailabilityRule",
                table: "AvailabilityRule",
                column: "ID");

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

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityRule_Employee_Date",
                table: "AvailabilityRule",
                columns: new[] { "Employee", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilityRule_AvailabilityDay",
                table: "AvailabilityRule",
                columns: new[] { "Employee", "Date" },
                principalTable: "AvailabilityDay",
                principalColumns: new[] { "Employee", "Date" });
        }
    }
}
