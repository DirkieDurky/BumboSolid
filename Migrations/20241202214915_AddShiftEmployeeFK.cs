using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftEmployeeFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Shift_Employee",
                table: "Shift",
                column: "Employee");

            migrationBuilder.AddForeignKey(
                name: "FK_Shift_Employee",
                table: "Shift",
                column: "Employee",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shift_Employee",
                table: "Shift");

            migrationBuilder.DropIndex(
                name: "IX_Shift_Employee",
                table: "Shift");
        }
    }
}
