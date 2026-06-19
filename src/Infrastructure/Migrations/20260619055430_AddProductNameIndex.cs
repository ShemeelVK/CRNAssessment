using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRNAssessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductName",
                table: "Product",
                column: "ProductName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_ProductName",
                table: "Product");
        }
    }
}
