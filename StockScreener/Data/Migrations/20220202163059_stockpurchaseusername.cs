using Microsoft.EntityFrameworkCore.Migrations;

namespace StockScreener.Data.Migrations
{
    public partial class stockpurchaseusername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "StockPurchase",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "StockPurchase");
        }
    }
}
