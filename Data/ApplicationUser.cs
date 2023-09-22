using Microsoft.AspNetCore.Identity;

namespace Auction.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? ProfilePicture { get; set; }
        public string? userdef { get; set; }
    }
}
