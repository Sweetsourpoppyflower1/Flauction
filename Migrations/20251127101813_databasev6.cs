using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class databasev6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "s_password",
                table: "Supplier",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "s_password",
                table: "Supplier");
        }
    }
}
