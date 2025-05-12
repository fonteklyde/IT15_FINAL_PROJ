using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT15_Final_Proj.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackingIdToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingId",
                table: "Orders");
        }
    }
}
