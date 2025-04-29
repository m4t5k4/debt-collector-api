using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace debt_collector_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaryCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryCurrency",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryCurrency",
                table: "Persons");
        }
    }
}
