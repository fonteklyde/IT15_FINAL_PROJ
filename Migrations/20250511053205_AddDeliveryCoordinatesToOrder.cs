using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
//asdasd
namespace IT15_Final_Proj.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryCoordinatesToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingId",
                table: "Orders");

            migrationBuilder.AddColumn<double>(
                name: "DeliveryLatitude",
                table: "Orders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DeliveryLongitude",
                table: "Orders",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryLatitude",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryLongitude",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "TrackingId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
