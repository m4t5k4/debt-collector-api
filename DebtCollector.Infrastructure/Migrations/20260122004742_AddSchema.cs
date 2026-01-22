using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DebtCollector.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DebtCollector");

            migrationBuilder.CreateTable(
                name: "Images",
                schema: "DebtCollector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "DebtCollector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "DebtCollector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    CreatedByPersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByPersonId = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "DebtCollector",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                schema: "DebtCollector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    PrimaryCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    EmailVerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailVerificationSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByPersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByPersonId = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "DebtCollector",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                schema: "DebtCollector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    TotalOrdersCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedByPersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByPersonId = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "DebtCollector",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "DebtCollector",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PersonGroups",
                schema: "DebtCollector",
                columns: table => new
                {
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedByPersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByPersonId = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonGroups", x => new { x.PersonId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_PersonGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "DebtCollector",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonGroups_Persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "DebtCollector",
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "DebtCollector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    CreatedByPersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByPersonId = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalSchema: "DebtCollector",
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "DebtCollector",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "DebtCollector",
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Debtors",
                schema: "DebtCollector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HasPaid = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByPersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByPersonId = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debtors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Debtors_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "DebtCollector",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Debtors_Persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "DebtCollector",
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payers",
                schema: "DebtCollector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedByPersonId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByPersonId = table.Column<int>(type: "int", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payers_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "DebtCollector",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payers_Persons_PersonId",
                        column: x => x.PersonId,
                        principalSchema: "DebtCollector",
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Debtors_OrderId",
                schema: "DebtCollector",
                table: "Debtors",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Debtors_PersonId",
                schema: "DebtCollector",
                table: "Debtors",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_GroupId",
                schema: "DebtCollector",
                table: "Expenses",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ImageId",
                schema: "DebtCollector",
                table: "Expenses",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ImageId",
                schema: "DebtCollector",
                table: "Groups",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ExpenseId",
                schema: "DebtCollector",
                table: "Orders",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ImageId",
                schema: "DebtCollector",
                table: "Orders",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PersonId",
                schema: "DebtCollector",
                table: "Orders",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Payers_OrderId",
                schema: "DebtCollector",
                table: "Payers",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payers_PersonId",
                schema: "DebtCollector",
                table: "Payers",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonGroups_GroupId",
                schema: "DebtCollector",
                table: "PersonGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Email",
                schema: "DebtCollector",
                table: "Persons",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ImageId",
                schema: "DebtCollector",
                table: "Persons",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debtors",
                schema: "DebtCollector");

            migrationBuilder.DropTable(
                name: "Payers",
                schema: "DebtCollector");

            migrationBuilder.DropTable(
                name: "PersonGroups",
                schema: "DebtCollector");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "DebtCollector");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "DebtCollector");

            migrationBuilder.DropTable(
                name: "Expenses",
                schema: "DebtCollector");

            migrationBuilder.DropTable(
                name: "Persons",
                schema: "DebtCollector");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "DebtCollector");

            migrationBuilder.DropTable(
                name: "Images",
                schema: "DebtCollector");
        }
    }
}
