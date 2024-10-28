using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStaffColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Staff",
                table: "PrognosisFunction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Staff",
                table: "PrognosisFunction",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
