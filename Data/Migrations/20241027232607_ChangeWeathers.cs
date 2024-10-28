using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWeathers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)0,
                column: "Impact",
                value: (short)75);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)1,
                column: "Impact",
                value: (short)50);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)2,
                column: "Impact",
                value: (short)25);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)3,
                column: "Impact",
                value: (short)0);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)4,
                column: "Impact",
                value: (short)-25);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)5,
                column: "Impact",
                value: (short)-50);

            migrationBuilder.InsertData(
                table: "Weather",
                columns: new[] { "ID", "Impact" },
                values: new object[] { (byte)6, (short)-75 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)6);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)0,
                column: "Impact",
                value: (short)0);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)1,
                column: "Impact",
                value: (short)20);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)2,
                column: "Impact",
                value: (short)40);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)3,
                column: "Impact",
                value: (short)60);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)4,
                column: "Impact",
                value: (short)80);

            migrationBuilder.UpdateData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)5,
                column: "Impact",
                value: (short)100);
        }
    }
}
