using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Capability_Department",
                table: "Capability");

            migrationBuilder.DropForeignKey(
                name: "FK_Capability_Employee",
                table: "Capability");

            migrationBuilder.AddForeignKey(
                name: "FK_Capability_Department",
                table: "Capability",
                column: "Department",
                principalTable: "Department",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Capability_Employee",
                table: "Capability",
                column: "Employee",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Capability_Department",
                table: "Capability");

            migrationBuilder.DropForeignKey(
                name: "FK_Capability_Employee",
                table: "Capability");

            migrationBuilder.AddForeignKey(
                name: "FK_Capability_Department",
                table: "Capability",
                column: "Department",
                principalTable: "Department",
                principalColumn: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Capability_Employee",
                table: "Capability",
                column: "Employee",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
