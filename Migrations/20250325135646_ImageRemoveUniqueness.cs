using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace debt_collector_api.Migrations
{
    /// <inheritdoc />
    public partial class ImageRemoveUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ImageId",
                table: "Persons",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ImageId",
                table: "Orders",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ImageId",
                table: "Groups",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ImageId",
                table: "Expenses",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
