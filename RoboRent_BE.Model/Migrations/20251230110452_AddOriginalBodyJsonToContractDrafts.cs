using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoboRent_BE.Model.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalBodyJsonToContractDrafts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalBodyJson",
                table: "ContractDrafts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalBodyJson",
                table: "ContractDrafts");
        }
    }
}
