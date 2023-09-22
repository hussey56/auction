namespace Auction.Services
{
    public interface IAuctionServices
    {
        Tuple<int, string> SaveImage(IFormFile imageFile);
        public bool DeleteImage(string ImageFileName);
    }
}
