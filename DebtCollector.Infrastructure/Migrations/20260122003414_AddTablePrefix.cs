using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DebtCollector.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTablePrefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DebtCollector_Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtCollector_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DebtCollector_RefreshTokens",
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
                    table.PrimaryKey("PK_DebtCollector_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DebtCollector_Groups",
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
                    table.PrimaryKey("PK_DebtCollector_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Groups_DebtCollector_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "DebtCollector_Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DebtCollector_Persons",
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
                    table.PrimaryKey("PK_DebtCollector_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Persons_DebtCollector_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "DebtCollector_Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DebtCollector_Expenses",
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
                    table.PrimaryKey("PK_DebtCollector_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Expenses_DebtCollector_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "DebtCollector_Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Expenses_DebtCollector_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "DebtCollector_Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DebtCollector_PersonGroups",
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
                    table.PrimaryKey("PK_DebtCollector_PersonGroups", x => new { x.PersonId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_DebtCollector_PersonGroups_DebtCollector_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "DebtCollector_Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebtCollector_PersonGroups_DebtCollector_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "DebtCollector_Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebtCollector_Orders",
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
                    table.PrimaryKey("PK_DebtCollector_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Orders_DebtCollector_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "DebtCollector_Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Orders_DebtCollector_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "DebtCollector_Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Orders_DebtCollector_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "DebtCollector_Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DebtCollector_Debtors",
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
                    table.PrimaryKey("PK_DebtCollector_Debtors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Debtors_DebtCollector_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "DebtCollector_Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Debtors_DebtCollector_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "DebtCollector_Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DebtCollector_Payers",
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
                    table.PrimaryKey("PK_DebtCollector_Payers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Payers_DebtCollector_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "DebtCollector_Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DebtCollector_Payers_DebtCollector_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "DebtCollector_Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Debtors_OrderId",
                table: "DebtCollector_Debtors",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Debtors_PersonId",
                table: "DebtCollector_Debtors",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Expenses_GroupId",
                table: "DebtCollector_Expenses",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Expenses_ImageId",
                table: "DebtCollector_Expenses",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Groups_ImageId",
                table: "DebtCollector_Groups",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Orders_ExpenseId",
                table: "DebtCollector_Orders",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Orders_ImageId",
                table: "DebtCollector_Orders",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Orders_PersonId",
                table: "DebtCollector_Orders",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Payers_OrderId",
                table: "DebtCollector_Payers",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Payers_PersonId",
                table: "DebtCollector_Payers",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_PersonGroups_GroupId",
                table: "DebtCollector_PersonGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Persons_Email",
                table: "DebtCollector_Persons",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebtCollector_Persons_ImageId",
                table: "DebtCollector_Persons",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebtCollector_Debtors");

            migrationBuilder.DropTable(
                name: "DebtCollector_Payers");

            migrationBuilder.DropTable(
                name: "DebtCollector_PersonGroups");

            migrationBuilder.DropTable(
                name: "DebtCollector_RefreshTokens");

            migrationBuilder.DropTable(
                name: "DebtCollector_Orders");

            migrationBuilder.DropTable(
                name: "DebtCollector_Expenses");

            migrationBuilder.DropTable(
                name: "DebtCollector_Persons");

            migrationBuilder.DropTable(
                name: "DebtCollector_Groups");

            migrationBuilder.DropTable(
                name: "DebtCollector_Images");
        }
    }
}
