using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BumboSolid.Migrations
{
    /// <inheritdoc />
    public partial class RenameEmployeeToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_Employee_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_Employee_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_Employee_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_Employee_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilityRule_Employee",
                table: "AvailabilityRule");

            migrationBuilder.DropForeignKey(
                name: "FK_Capability_Employee",
                table: "Capability");

            migrationBuilder.DropForeignKey(
                name: "FK_FillRequest_Employee",
                table: "FillRequest");

            migrationBuilder.DropTable(
                name: "Employee");

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

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_User_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_User_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_User_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_User_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilityRule_Employee",
                table: "AvailabilityRule",
                column: "Employee",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Capability_Employee",
                table: "Capability",
                column: "Employee",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FillRequest_Employee",
                table: "FillRequest",
                column: "SubstituteEmployeeID",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_User_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_User_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_User_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_User_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilityRule_Employee",
                table: "AvailabilityRule");

            migrationBuilder.DropForeignKey(
                name: "FK_Capability_Employee",
                table: "Capability");

            migrationBuilder.DropForeignKey(
                name: "FK_FillRequest_Employee",
                table: "FillRequest");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    EmployedSince = table.Column<DateOnly>(type: "date", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: false),
                    LastName = table.Column<string>(type: "varchar(90)", unicode: false, maxLength: 90, nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PlaceOfResidence = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetName = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    StreetNumber = table.Column<int>(type: "int", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Employee",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Employee",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_Employee_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_Employee_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Employee_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_Employee_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilityRule_Employee",
                table: "AvailabilityRule",
                column: "Employee",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Capability_Employee",
                table: "Capability",
                column: "Employee",
                principalTable: "Employee",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FillRequest_Employee",
                table: "FillRequest",
                column: "SubstituteEmployeeID",
                principalTable: "Employee",
                principalColumn: "Id");
        }
    }
}
