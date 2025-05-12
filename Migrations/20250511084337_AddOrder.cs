using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT15_Final_Proj.Migrations
{
    /// <inheritdoc />
    public partial class AddOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DeliveryLocations_OrderId",
                table: "DeliveryLocations",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryLocations_Orders_OrderId",
                table: "DeliveryLocations",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryLocations_Orders_OrderId",
                table: "DeliveryLocations");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryLocations_OrderId",
                table: "DeliveryLocations");
        }
    }
}
