using Microsoft.EntityFrameworkCore.Migrations;

namespace StockScreener.Data.Migrations
{
    public partial class WinLoss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "WinLoss",
                table: "StockPurchase",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinLoss",
                table: "StockPurchase");
        }
    }
}
