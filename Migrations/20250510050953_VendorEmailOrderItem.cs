using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT15_Final_Proj.Migrations
{
    /// <inheritdoc />
    public partial class VendorEmailOrderItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VendorEmail",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VendorEmail",
                table: "OrderItems");
        }
    }
}
