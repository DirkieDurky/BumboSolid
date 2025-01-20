using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class CLASurchargEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HolidaySurcharge",
                table: "CLAEntry",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CLASurchargeEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CLAEntryId = table.Column<int>(type: "int", nullable: false),
                    Surcharge = table.Column<int>(type: "int", nullable: false),
                    Weekday = table.Column<byte>(type: "tinyint", nullable: true),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLASurchargeEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CLASurchargeEntry_CLAEntry",
                        column: x => x.CLAEntryId,
                        principalTable: "CLAEntry",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CLASurchargeEntry_CLAEntryId",
                table: "CLASurchargeEntry",
                column: "CLAEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CLASurchargeEntry");

            migrationBuilder.DropColumn(
                name: "HolidaySurcharge",
                table: "CLAEntry");
        }
    }
}
