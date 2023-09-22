using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auction.Data.Migrations
{
    /// <inheritdoc />
    public partial class forbiddin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionItemId",
                table: "Bids");

            migrationBuilder.AddColumn<DateTime>(
                name: "BidTime",
                table: "Bids",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BidderIdId",
                table: "Bids",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bids_BidderIdId",
                table: "Bids",
                column: "BidderIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_AspNetUsers_BidderIdId",
                table: "Bids",
                column: "BidderIdId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_AspNetUsers_BidderIdId",
                table: "Bids");

            migrationBuilder.DropIndex(
                name: "IX_Bids_BidderIdId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "BidTime",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "BidderIdId",
                table: "Bids");

            migrationBuilder.AddColumn<int>(
                name: "AuctionItemId",
                table: "Bids",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
