using Auction.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Auction.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    public  DbSet<Category>Categories { get; set; }
      public  DbSet<Messages> Messages { get; set; }
        public DbSet<AuctionGood> AuctionGoods { get; set; }
        public DbSet<Biddings> Bids { get; set; }
       


    }
}