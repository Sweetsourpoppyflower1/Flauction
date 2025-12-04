using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class databasev18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "vat",
                table: "Company",
                newName: "c_vat");

            migrationBuilder.RenameColumn(
                name: "postalcode",
                table: "Company",
                newName: "c_postalcode");

            migrationBuilder.RenameColumn(
                name: "iban",
                table: "Company",
                newName: "c_iban");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "Company",
                newName: "c_country");

            migrationBuilder.RenameColumn(
                name: "bicswift",
                table: "Company",
                newName: "c_bicswift");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "Company",
                newName: "c_address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "c_vat",
                table: "Company",
                newName: "vat");

            migrationBuilder.RenameColumn(
                name: "c_postalcode",
                table: "Company",
                newName: "postalcode");

            migrationBuilder.RenameColumn(
                name: "c_iban",
                table: "Company",
                newName: "iban");

            migrationBuilder.RenameColumn(
                name: "c_country",
                table: "Company",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "c_bicswift",
                table: "Company",
                newName: "bicswift");

            migrationBuilder.RenameColumn(
                name: "c_address",
                table: "Company",
                newName: "address");
        }
    }
}
