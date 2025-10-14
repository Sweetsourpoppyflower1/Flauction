using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gebruiker",
                columns: table => new
                {
                    GebruikersID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gebruikersnaam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Wachtwoord = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gebruiker", x => x.GebruikersID);
                });

            migrationBuilder.CreateTable(
                name: "Veiling",
                columns: table => new
                {
                    VeilingsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTijd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EindTijd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VeilingmeesterID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiling", x => x.VeilingsID);
                    table.ForeignKey(
                        name: "FK_Veiling_Gebruiker_VeilingmeesterID",
                        column: x => x.VeilingmeesterID,
                        principalTable: "Gebruiker",
                        principalColumn: "GebruikersID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dashboard",
                columns: table => new
                {
                    DashboardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeilingsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboard", x => x.DashboardID);
                    table.ForeignKey(
                        name: "FK_Dashboard_Veiling_VeilingsID",
                        column: x => x.VeilingsID,
                        principalTable: "Veiling",
                        principalColumn: "VeilingsID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Veilingsproduct",
                columns: table => new
                {
                    VeilingsproductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Hoeveelheid = table.Column<int>(type: "int", nullable: false),
                    Kenmerken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Aanvoerder = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Prijs = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    VeilingsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veilingsproduct", x => x.VeilingsproductID);
                    table.ForeignKey(
                        name: "FK_Veilingsproduct_Veiling_VeilingsID",
                        column: x => x.VeilingsID,
                        principalTable: "Veiling",
                        principalColumn: "VeilingsID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bod",
                columns: table => new
                {
                    BodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bieder = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bedrag = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    VeilingsproductID = table.Column<int>(type: "int", nullable: false),
                    Tijdstip = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GebruikersID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bod", x => x.BodID);
                    table.ForeignKey(
                        name: "FK_Bod_Gebruiker_GebruikersID",
                        column: x => x.GebruikersID,
                        principalTable: "Gebruiker",
                        principalColumn: "GebruikersID");
                    table.ForeignKey(
                        name: "FK_Bod_Veilingsproduct_VeilingsproductID",
                        column: x => x.VeilingsproductID,
                        principalTable: "Veilingsproduct",
                        principalColumn: "VeilingsproductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bod_GebruikersID",
                table: "Bod",
                column: "GebruikersID");

            migrationBuilder.CreateIndex(
                name: "IX_Bod_VeilingsproductID",
                table: "Bod",
                column: "VeilingsproductID");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboard_VeilingsID",
                table: "Dashboard",
                column: "VeilingsID");

            migrationBuilder.CreateIndex(
                name: "IX_Veiling_VeilingmeesterID",
                table: "Veiling",
                column: "VeilingmeesterID");

            migrationBuilder.CreateIndex(
                name: "IX_Veilingsproduct_VeilingsID",
                table: "Veilingsproduct",
                column: "VeilingsID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bod");

            migrationBuilder.DropTable(
                name: "Dashboard");

            migrationBuilder.DropTable(
                name: "Veilingsproduct");

            migrationBuilder.DropTable(
                name: "Veiling");

            migrationBuilder.DropTable(
                name: "Gebruiker");
        }
    }
}
