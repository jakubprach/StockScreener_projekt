using Microsoft.EntityFrameworkCore.Migrations;

namespace StockScreener.Data.Migrations
{
    public partial class addStockPurchase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockPurchase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoughtAt = table.Column<double>(type: "float", nullable: false),
                    StockIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SharesQuantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPurchase", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockPurchase");
        }
    }
}
