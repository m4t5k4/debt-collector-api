using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace debt_collector_api.Migrations
{
    /// <inheritdoc />
    public partial class Images : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Persons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Images",
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

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ImageId",
                table: "Persons",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ImageId",
                table: "Orders",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ImageId",
                table: "Groups",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ImageId",
                table: "Expenses",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Images_ImageId",
                table: "Expenses",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Images_ImageId",
                table: "Groups",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Images_ImageId",
                table: "Orders",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Images_ImageId",
                table: "Persons",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Images_ImageId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Images_ImageId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Images_ImageId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Images_ImageId",
                table: "Persons");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Persons_ImageId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ImageId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ImageId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ImageId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Expenses");
        }
    }
}
