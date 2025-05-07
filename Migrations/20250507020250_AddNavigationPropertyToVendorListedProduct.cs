using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT15_Final_Proj.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationPropertyToVendorListedProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VendorListedProducts_ProductId",
                table: "VendorListedProducts",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorListedProducts_Products_ProductId",
                table: "VendorListedProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorListedProducts_Products_ProductId",
                table: "VendorListedProducts");

            migrationBuilder.DropIndex(
                name: "IX_VendorListedProducts_ProductId",
                table: "VendorListedProducts");
        }
    }
}
