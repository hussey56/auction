using Auction.Data;
using Auction.Data.Migrations;
using System.ComponentModel.DataAnnotations;

namespace Auction.Models
{
    public class Biddings
    {
        [Key]
        public int BidId { get; set; }

        public DateTime BidTime { get; set; }

        public decimal BidAmount { get; set; }
        public string Bidder { get; set; }
        public int goodid { get; set; }

        public ApplicationUser BidderId { get; set; }
        public virtual AuctionGood AuctionGood { get; set; }
    }
}
