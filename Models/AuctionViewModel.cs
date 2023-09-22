namespace Auction.Models
{
    public class AuctionViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int CurrentBid { get; set; }
        public int SellerId { get; set; }
        public int CategoryId { get; set; }
        public string? Picture { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
