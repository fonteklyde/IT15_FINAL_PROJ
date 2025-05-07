using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IT15_Final_Proj.Migrations
{
    /// <inheritdoc />
<<<<<<<< HEAD:Migrations/20250507131259_RemoveCartItem.cs
    public partial class RemoveCartItem : Migration
========
    public partial class AddVendorListedProductTable : Migration
>>>>>>>> f5eba88a301d00a23414c003e71117fdbaa4cd44:Migrations/20250507014521_AddVendorListedProductTable.cs
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
<<<<<<<< HEAD:Migrations/20250507131259_RemoveCartItem.cs
                name: "CartItems",
========
                name: "VendorListedProducts",
>>>>>>>> f5eba88a301d00a23414c003e71117fdbaa4cd44:Migrations/20250507014521_AddVendorListedProductTable.cs
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    VendorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
<<<<<<<< HEAD:Migrations/20250507131259_RemoveCartItem.cs
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    VendorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                });
        }
========
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorListedProducts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorListedProducts");
        }
>>>>>>>> f5eba88a301d00a23414c003e71117fdbaa4cd44:Migrations/20250507014521_AddVendorListedProductTable.cs
    }
}
