using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace debt_collector_api.Migrations
{
    /// <inheritdoc />
    public partial class renameTablesPlural : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expense_Group_GroupId",
                table: "Expense");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Expense_ExpenseId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Person_PersonId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonGroup_Group_GroupId",
                table: "PersonGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonGroup_Person_PersonId",
                table: "PersonGroup");

            migrationBuilder.DropTable(
                name: "PaidBy");

            migrationBuilder.DropTable(
                name: "Share");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonGroup",
                table: "PersonGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Person",
                table: "Person");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Expense",
                table: "Expense");

            migrationBuilder.RenameTable(
                name: "PersonGroup",
                newName: "PersonGroups");

            migrationBuilder.RenameTable(
                name: "Person",
                newName: "Persons");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameTable(
                name: "Expense",
                newName: "Expenses");

            migrationBuilder.RenameIndex(
                name: "IX_PersonGroup_GroupId",
                table: "PersonGroups",
                newName: "IX_PersonGroups_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_PersonId",
                table: "Orders",
                newName: "IX_Orders_PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_ExpenseId",
                table: "Orders",
                newName: "IX_Orders_ExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_Expense_GroupId",
                table: "Expenses",
                newName: "IX_Expenses_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonGroups",
                table: "PersonGroups",
                columns: new[] { "PersonId", "GroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Persons",
                table: "Persons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Expenses",
                table: "Expenses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Debtors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debtors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Debtors_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debtors_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByPersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedByPersonId = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payers_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debtors_OrderId",
                table: "Debtors",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Debtors_PersonId",
                table: "Debtors",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Payers_OrderId",
                table: "Payers",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payers_PersonId",
                table: "Payers",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Groups_GroupId",
                table: "Expenses",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Expenses_ExpenseId",
                table: "Orders",
                column: "ExpenseId",
                principalTable: "Expenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Persons_PersonId",
                table: "Orders",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonGroups_Groups_GroupId",
                table: "PersonGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonGroups_Persons_PersonId",
                table: "PersonGroups",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Groups_GroupId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Expenses_ExpenseId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Persons_PersonId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonGroups_Groups_GroupId",
                table: "PersonGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonGroups_Persons_PersonId",
                table: "PersonGroups");

            migrationBuilder.DropTable(
                name: "Debtors");

            migrationBuilder.DropTable(
                name: "Payers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Persons",
                table: "Persons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonGroups",
                table: "PersonGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Expenses",
                table: "Expenses");

            migrationBuilder.RenameTable(
                name: "Persons",
                newName: "Person");

            migrationBuilder.RenameTable(
                name: "PersonGroups",
                newName: "PersonGroup");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameTable(
                name: "Expenses",
                newName: "Expense");

            migrationBuilder.RenameIndex(
                name: "IX_PersonGroups_GroupId",
                table: "PersonGroup",
                newName: "IX_PersonGroup_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_PersonId",
                table: "Order",
                newName: "IX_Order_PersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ExpenseId",
                table: "Order",
                newName: "IX_Order_ExpenseId");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_GroupId",
                table: "Expense",
                newName: "IX_Expense_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Person",
                table: "Person",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonGroup",
                table: "PersonGroup",
                columns: new[] { "PersonId", "GroupId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Expense",
                table: "Expense",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PaidBy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedByPersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpenseId = table.Column<int>(type: "int", nullable: true),
                    ModifiedByPersonId = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaidBy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaidBy_Expense_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expense",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaidBy_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaidBy_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Share",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Share", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Share_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Share_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaidBy_ExpenseId",
                table: "PaidBy",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_PaidBy_OrderId",
                table: "PaidBy",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaidBy_PersonId",
                table: "PaidBy",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Share_OrderId",
                table: "Share",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Share_PersonId",
                table: "Share",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expense_Group_GroupId",
                table: "Expense",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Expense_ExpenseId",
                table: "Order",
                column: "ExpenseId",
                principalTable: "Expense",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Person_PersonId",
                table: "Order",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonGroup_Group_GroupId",
                table: "PersonGroup",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonGroup_Person_PersonId",
                table: "PersonGroup",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
