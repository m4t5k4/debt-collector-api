using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace debt_collector_api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExpenseParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseParticipant");

            migrationBuilder.AddColumn<int>(
                name: "TotalCost",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Order");

            migrationBuilder.CreateTable(
                name: "ExpenseParticipant",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseParticipant", x => new { x.ExpenseId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_ExpenseParticipant_Expense_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expense",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpenseParticipant_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseParticipant_PersonId",
                table: "ExpenseParticipant",
                column: "PersonId");
        }
    }
}
