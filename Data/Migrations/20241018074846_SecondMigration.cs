using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BumboSolid.Data.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                table: "Function",
                column: "Name",
                values: new object[]
                {
                    "Kassa",
                    "Vakkenvuller",
                    "Vers"
                });

            migrationBuilder.InsertData(
                table: "Weather",
                columns: new[] { "ID", "Impact" },
                values: new object[,]
                {
                    { (byte)0, (short)0 },
                    { (byte)1, (short)20 },
                    { (byte)2, (short)40 },
                    { (byte)3, (short)60 },
                    { (byte)4, (short)80 },
                    { (byte)5, (short)100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FactorType",
                keyColumn: "Type",
                keyValue: "Feestdagen");

            migrationBuilder.DeleteData(
                table: "FactorType",
                keyColumn: "Type",
                keyValue: "Overig");

            migrationBuilder.DeleteData(
                table: "FactorType",
                keyColumn: "Type",
                keyValue: "Weer");

            migrationBuilder.DeleteData(
                table: "Function",
                keyColumn: "Name",
                keyValue: "Kassa");

            migrationBuilder.DeleteData(
                table: "Function",
                keyColumn: "Name",
                keyValue: "Vakkenvuller");

            migrationBuilder.DeleteData(
                table: "Function",
                keyColumn: "Name",
                keyValue: "Vers");

            migrationBuilder.DeleteData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)0);

            migrationBuilder.DeleteData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)4);

            migrationBuilder.DeleteData(
                table: "Weather",
                keyColumn: "ID",
                keyValue: (byte)5);
        }
    }
}
