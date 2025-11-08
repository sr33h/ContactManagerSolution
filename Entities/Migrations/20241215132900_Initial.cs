using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceiveNewsLetters = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonID);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryID", "CountryName" },
                values: new object[,]
                {
                    { new Guid("01ef672f-5aef-4c0c-8474-babf107220fd"), "UK" },
                    { new Guid("920f3264-4d96-4245-9f2f-7ff14d766887"), "Germany" },
                    { new Guid("a2053bf3-7aaa-4c77-a23d-8fbbea341754"), "Russia" },
                    { new Guid("d8c07225-bc6a-4b15-b520-a1f7131f46f6"), "USA" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonID", "Address", "CountryID", "DateOfBirth", "Email", "Gender", "PersonName", "ReceiveNewsLetters" },
                values: new object[,]
                {
                    { new Guid("9219bbdd-d30c-45f4-929f-024d9904f188"), "address3", new Guid("01ef672f-5aef-4c0c-8474-babf107220fd"), new DateTime(1999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "johndoe3@gmail.com", "Female", "John Doe3", true },
                    { new Guid("a7ab253e-05c7-4bf7-93aa-22e551124171"), "address1", new Guid("920f3264-4d96-4245-9f2f-7ff14d766887"), new DateTime(1999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "johndoe1@gmail.com", "Female", "John Doe1", true },
                    { new Guid("ed9623aa-e92c-4170-8c76-f0660d783f05"), "address2", new Guid("d8c07225-bc6a-4b15-b520-a1f7131f46f6"), new DateTime(1999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "johndoe2@gmail.com", "Other", "John Doe2", true },
                    { new Guid("fdb0ae15-1d53-460d-9352-75a67293c731"), "address4", new Guid("a2053bf3-7aaa-4c77-a23d-8fbbea341754"), new DateTime(1999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "johndoe4@gmail.com", "Male", "John Doe4", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
