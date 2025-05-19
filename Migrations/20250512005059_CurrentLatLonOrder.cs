using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
//asdasd
namespace IT15_Final_Proj.Migrations
{
    /// <inheritdoc />
    public partial class CurrentLatLonOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CurrentLat",
                table: "Orders",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLon",
                table: "Orders",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentLat",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrentLon",
                table: "Orders");
        }
    }
}
