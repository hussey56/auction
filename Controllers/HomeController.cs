using Auction.Data;
using Auction.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace Auction.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _auctionContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ApplicationDbContext auctionContext, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _auctionContext = auctionContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            DateTime currentDateTime = DateTime.Now;

            if (User.Identity.IsAuthenticated)
            {
                var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser.userdef != "active")
                {
                    return RedirectToAction("Blocked","Auctions");
                }

                var cars = _auctionContext.AuctionGoods.Where(b => b.Seller.Id != currentUser.Id && b.EndTime > currentDateTime && b.Seller.userdef == "active" && b.CategoryId == 8).Take(3).ToList();

                ViewBag.Cars = cars;
                var jewellery = _auctionContext.AuctionGoods.Where(b => b.Seller.Id != currentUser.Id && b.EndTime > currentDateTime && b.Seller.userdef == "active" && b.CategoryId == 9).Take(3).ToList();

                ViewBag.Jewellery = jewellery;
                var watch = _auctionContext.AuctionGoods.Where(b => b.Seller.Id != currentUser.Id && b.EndTime > currentDateTime && b.Seller.userdef == "active" && b.CategoryId == 10).Take(3).ToList();

                ViewBag.watch = watch;
                var state = _auctionContext.AuctionGoods.Where(b => b.Seller.Id != currentUser.Id && b.EndTime > currentDateTime && b.Seller.userdef == "active" && b.CategoryId == 7).Take(3).ToList();
                ViewBag.state = state;
                var auctionGoodWithMostBids = _auctionContext.AuctionGoods.Where(b => b.Seller.Id != currentUser.Id && b.EndTime > currentDateTime && b.Seller.userdef == "active").OrderByDescending(b=>b.StartTime).Take(6).ToList();
                ViewBag.popular = auctionGoodWithMostBids;

                return View();

            }
            else
            {
                var state = _auctionContext.AuctionGoods.Where(b => b.EndTime > currentDateTime && b.Seller.userdef == "active" && b.CategoryId == 7).Take(3).ToList();

                ViewBag.state = state;
                var cars = _auctionContext.AuctionGoods.Where(b=> b.EndTime > currentDateTime && b.Seller.userdef == "active" && b.CategoryId == 8).Take(3).ToList();

                ViewBag.Cars = cars;
                var jewellery = _auctionContext.AuctionGoods.Where(b => b.EndTime > currentDateTime && b.Seller.userdef == "active" && b.CategoryId == 9).Take(3).ToList();

                ViewBag.Jewellery = jewellery;
                var watch = _auctionContext.AuctionGoods.Where(b =>  b.EndTime > currentDateTime && b.Seller.userdef == "active" && b.CategoryId == 10).Take(3).ToList();

                ViewBag.watch = watch;
                // Get the list of all auction goods.
                var auctionGoods = _auctionContext.AuctionGoods.Where(b=> b.EndTime > currentDateTime && b.Seller.userdef == "active").ToList();

                // Group the auction goods by their ID.
                var auctionGoodWithMostBids = _auctionContext.AuctionGoods.Where(b=> b.EndTime > currentDateTime && b.Seller.userdef == "active").OrderByDescending(b => b.StartTime).Take(6).ToList();

                ViewBag.popular = auctionGoodWithMostBids;

                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}