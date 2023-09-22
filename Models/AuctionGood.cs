using Auction.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Auction.Models
{
    public class AuctionGood
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ApplicationUser Seller { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string? Picture { get; set; }

        public virtual ICollection<Biddings> Bids { get; set; }
    }
}
