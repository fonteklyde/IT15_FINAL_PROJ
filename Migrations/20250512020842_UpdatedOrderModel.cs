using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
//asdasd
namespace IT15_Final_Proj.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedOrderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncodedRoutePoints",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteStepIndex",
                table: "Orders",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncodedRoutePoints",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RouteStepIndex",
                table: "Orders");
        }
    }
}
