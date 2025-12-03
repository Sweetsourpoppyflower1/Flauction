using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flauction.Migrations
{
    /// <inheritdoc />
    public partial class databasev5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "min_price",
                table: "Auction",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "min_price",
                table: "Auction");
        }
    }
}
