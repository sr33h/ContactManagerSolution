using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class FluentAPIForTin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TIN",
                table: "Persons",
                newName: "TaxIdentificationNumber");

            migrationBuilder.AlterColumn<string>(
                name: "TaxIdentificationNumber",
                table: "Persons",
                type: "varchar(8)",
                nullable: true,
                defaultValue: "ABC12345",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("9219bbdd-d30c-45f4-929f-024d9904f188"),
                column: "TaxIdentificationNumber",
                value: "ABC12345");

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("a7ab253e-05c7-4bf7-93aa-22e551124171"),
                column: "TaxIdentificationNumber",
                value: "ABC12345");

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("ed9623aa-e92c-4170-8c76-f0660d783f05"),
                column: "TaxIdentificationNumber",
                value: "ABC12345");

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("fdb0ae15-1d53-460d-9352-75a67293c731"),
                column: "TaxIdentificationNumber",
                value: "ABC12345");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaxIdentificationNumber",
                table: "Persons",
                newName: "TIN");

            migrationBuilder.AlterColumn<string>(
                name: "TIN",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldNullable: true,
                oldDefaultValue: "ABC12345");

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("9219bbdd-d30c-45f4-929f-024d9904f188"),
                column: "TIN",
                value: null);

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("a7ab253e-05c7-4bf7-93aa-22e551124171"),
                column: "TIN",
                value: null);

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("ed9623aa-e92c-4170-8c76-f0660d783f05"),
                column: "TIN",
                value: null);

            migrationBuilder.UpdateData(
                table: "Persons",
                keyColumn: "PersonID",
                keyValue: new Guid("fdb0ae15-1d53-460d-9352-75a67293c731"),
                column: "TIN",
                value: null);
        }
    }
}
