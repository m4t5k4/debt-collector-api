using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace debt_collector_api.Migrations
{
    /// <inheritdoc />
    public partial class renameSharesAndPayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPaid",
                table: "Debtors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPaid",
                table: "Debtors");
        }
    }
}
