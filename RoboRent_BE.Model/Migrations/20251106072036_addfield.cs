using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoboRent_BE.Model.Migrations
{
    /// <inheritdoc />
    public partial class addfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerReason",
                table: "PriceQuotes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerReason",
                table: "PriceQuotes");
        }
    }
}
