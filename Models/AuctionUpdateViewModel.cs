namespace Auction.Models
{
    public class AuctionUpdateViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Picture { get; set; }
        public DateTime EndTime { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
