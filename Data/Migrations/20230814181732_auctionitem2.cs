using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auction.Data.Migrations
{
    /// <inheritdoc />
    public partial class auctionitem2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "AuctionItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SellerId1",
                table: "AuctionItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuctionItems_SellerId1",
                table: "AuctionItems",
                column: "SellerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionItems_AspNetUsers_SellerId1",
                table: "AuctionItems",
                column: "SellerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionItems_AspNetUsers_SellerId1",
                table: "AuctionItems");

            migrationBuilder.DropIndex(
                name: "IX_AuctionItems_SellerId1",
                table: "AuctionItems");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "AuctionItems");

            migrationBuilder.DropColumn(
                name: "SellerId1",
                table: "AuctionItems");
        }
    }
}
