using Auction.Data;
using Auction.Data.Migrations;
using Auction.Models;
using Auction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Auction.Controllers
{
    public class AuctionsController : Controller
    {
        private ApplicationDbContext _auctionContext;
        private readonly IAuctionServices _auctionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuctionsController(ApplicationDbContext auctionContext, IAuctionServices auctionService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _auctionContext = auctionContext;
            _auctionService = auctionService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<string> GetUsernameById(string userId)
        {
            // Get the user by their ID.
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Get the user's username.
                var username = user.Email;

                return username;
            }
            else
            {
                return "Email Not Found";
            }
           
        }
        public string GetName(string userid)
        {
            var  email = GetUsernameById(userid);

            return email.Result;
        }
        public async Task<string> GetProfileById(string userId)
        {
            // Get the user manager.


            // Get the user by their ID.
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Get the user's username.
                var username = user.ProfilePicture;
                if (username !="")
                {
                    return username;
                }
                else
                {
                    string another = "05.png";
                    return another ;
                }
               
            }
            else
            {
                return "Picture Not Found";
            }

        }
        public string GetPicture(string userid)
        {
            var email = GetProfileById(userid);

            return email.Result;
        }
        public AuctionGood GetAuctionTitle(int userid)
        {
            var email = _auctionContext.AuctionGoods.Where(b=>b.Id == userid).FirstOrDefault();
            if(email != null)
            {
                return email;
            }
            return null;

           
        }
        public static string ConvertNumber(decimal input)
        {
            if (input >= 1000000000) // Billion
            {
                return (input / 1000000000).ToString("0.##") + "B";
            }
            else if (input >= 1000000) // Million
            {
                return (input / 1000000).ToString("0.##") + "M";
            }
            else if (input >= 1000) // Thousand (K)
            {
                return (input / 1000).ToString("0.##") + "K";
            }
            else // Less than a thousand
            {
                return input.ToString("0.##");
            }
        }
        public int GetNumberOfBids(int auctionItemId)
        {
            // Get the bids for the auction item.
            var bids = _auctionContext.Bids.Where(b => b.AuctionGood.Id == auctionItemId && b.BidderId.userdef == "active").ToList();

            // Return the number of bids.
            return bids.Count;
        }
        public string GetTitleogItem(int auctionItemId)
        {
            // Get the bids for the auction item.
            var data = _auctionContext.AuctionGoods.Where(b => b.Id == auctionItemId).FirstOrDefault();

            // Return the number of bids.
            return data.Title;
        }
        public decimal GetStartingPrice(int auctionItemId)
        {
            // Get the bids for the auction item.
            var data = _auctionContext.AuctionGoods.Where(b => b.Id == auctionItemId).FirstOrDefault();

            // Return the number of bids.
            return data.StartingPrice;
        }
        public DateTime GetExpiry(int auctionItemId)
        {
            // Get the bids for the auction item.
            var data = _auctionContext.AuctionGoods.Where(b => b.Id == auctionItemId && b.Seller.userdef == "active").FirstOrDefault();

            // Return the number of bids.
            return data.EndTime;
        }
        public decimal GetHighestBidAmount(int auctionItemId)
        {
            // Get the bids for the auction item.
            var bids = _auctionContext.Bids.Where(b => b.AuctionGood.Id == auctionItemId && b.BidderId.userdef =="active").ToList();

            // Find the bid with the highest amount.
            var highestBid = bids.OrderByDescending(bid => bid.BidAmount).FirstOrDefault();

            // Return the amount of the highest bid.
            return highestBid?.BidAmount ?? 0;
        }
        public decimal GetLowestBidAmount(int auctionItemId)
        {
            // Get the bids for the auction item.
            var bids = _auctionContext.Bids.Where(b => b.AuctionGood.Id == auctionItemId && b.BidderId.userdef =="active").ToList();

            // Find the bid with the highest amount.
            var highestBid = bids.OrderBy(bid => bid.BidAmount).FirstOrDefault();

            // Return the amount of the highest bid.
            return highestBid?.BidAmount ?? 0;
        }
        public IQueryable<Biddings> GetBids(int auctionGoodId)
        {
            return this._auctionContext.Bids
                .Where(bid => bid.goodid == auctionGoodId && bid.BidderId.userdef == "active");
        }
        public int GetNumberOfUniqueBidders(int auctionGoodId)
        {
            var bids = this.GetBids(auctionGoodId);
            var uniqueBidders = new HashSet<string>();

            foreach (var bid in bids)
            {
                uniqueBidders.Add(bid.Bidder);
            }

            return uniqueBidders.Count;
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
                    return RedirectToAction("Blocked");
                }

                var data = _auctionContext.AuctionGoods.Where(b => b.Seller.Id != currentUser.Id && b.EndTime > currentDateTime && b.Seller.userdef == "active").ToList();
                return View(data);

            }  
            else
            {
                var data = _auctionContext.AuctionGoods.Where(b=>b.EndTime > currentDateTime && b.Seller.userdef == "active").ToList();
                return View(data);
            }
            
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> Adminindex()
        {
            DateTime currentDateTime = DateTime.Now;
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            var data = _auctionContext.AuctionGoods.Where(b =>b.EndTime <= currentDateTime).ToList();
                return View(data);

        }
        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            DateTime currentDateTime = DateTime.Now;
            if (User.Identity.IsAuthenticated)
            {
                var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser.userdef != "active")
                {
                    return RedirectToAction("Blocked");
                }
                // Get the auction items from the database.
                var auctionItems = _auctionContext.AuctionGoods.Where(auctionItem => auctionItem.Title.ToLower().Contains(searchText.ToLower()) && auctionItem.EndTime > currentDateTime && auctionItem.Seller.Id != currentUser.Id && auctionItem.Seller.userdef == "active").ToList();
                ViewData["tag"] = searchText;
                return View(auctionItems);
            }
            else
            {
                var auctionItems = _auctionContext.AuctionGoods.Where(auctionItem => auctionItem.Title.ToLower().Contains(searchText.ToLower()) && auctionItem.EndTime > currentDateTime && auctionItem.Seller.userdef =="active").ToList();
                ViewData["tag"] = searchText;
                return View(auctionItems);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked");
            }
            ViewBag.CategoryList = new SelectList(_auctionContext.Categories, "Id", "Name");
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuctionViewModel auctionItem)
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked");
            }
            if (ModelState.IsValid)
            {
                if (auctionItem.ImageFile != null)
                {
                    var result = _auctionService.SaveImage(auctionItem.ImageFile);
                    if (result.Item1 == 1)
                    {
                        var item = new AuctionGood()
                        {
                            Title = auctionItem.Title,
                            Description = auctionItem.Description,
                            StartingPrice = auctionItem.StartingPrice,
                            StartTime = auctionItem.StartTime,
                            EndTime = auctionItem.EndTime,
                            CategoryId = auctionItem.CategoryId,
                            Picture = result.Item2,
                            Seller = currentUser,

                        };
                        _auctionContext.AuctionGoods.Add(item);
                        _auctionContext.SaveChanges();
                        return RedirectToAction("Myauctions","User");
                    }
                }

            }
            ViewBag.CategoryList = new SelectList(_auctionContext.Categories, "Id", "Name", auctionItem.CategoryId);
            return RedirectToAction("Create");

        }
        [HttpGet]
        public async Task<IActionResult> CategoryGoods(int? id)
        {
            DateTime currentDateTime = DateTime.Now;

            if (User.Identity.IsAuthenticated)
            {
                var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
                var currentUser = await _userManager.GetUserAsync(User);
                var categoryname = _auctionContext.Categories.Where(b=>b.Id ==  id).FirstOrDefault();
                var name = categoryname.Name;
                ViewData["category"] = name;
                var data = _auctionContext.AuctionGoods.Where(b => b.Seller.Id != currentUser.Id && b.EndTime > currentDateTime && b.CategoryId == id && b.Seller.userdef =="active").ToList();
                return View(data);

            }
            else
            {
                var categoryname = _auctionContext.Categories.Where(b => b.Id == id).FirstOrDefault();
                var name = categoryname.Name;
                ViewData["category"] = name;
                var data = _auctionContext.AuctionGoods.Where(b => b.EndTime > currentDateTime && b.CategoryId == id && b.Seller.userdef =="active").ToList();
                return View(data);
            }
        }
        [HttpGet]
        public IActionResult Blocked()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
           
            if (id == null || _auctionContext.AuctionGoods == null)
            {
                return NotFound();
            }

            var item = await _auctionContext.AuctionGoods
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            if (User.Identity.IsAuthenticated)
            {
                if (currentUser.userdef != "active")
                {
                    return RedirectToAction("Blocked");
                }
                if (item.Seller == currentUser)
                {
                    return RedirectToAction("Index");
                }
          
                return View(item);
            }
            else
            {
                return View(item);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Updateauction(int? id)
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                if (currentUser.userdef != "active")
                {
                    return RedirectToAction("Blocked");
                }
          var item = _auctionContext.AuctionGoods.Where(b=>b.Id== id).FirstOrDefault();
               if(item.Seller.Id != currentUser.Id)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.CategoryList = new SelectList(_auctionContext.Categories, "Id", "Name");
                AuctionUpdateViewModel item2 = new AuctionUpdateViewModel();
                item2.Id = item.Id;
                return View(item2);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAuction(AuctionUpdateViewModel auction)
        {
         AuctionGood product = _auctionContext.AuctionGoods.Where(b=>b.Id == auction.Id).FirstOrDefault();

            if(product.Title != auction.Title)
            {
                product.Title = auction.Title;
            }
            if(auction.Description != null)
            {
                if (product.Description != auction.Description)
                {
                    product.Description = auction.Description;
                }
            }
           
            if (product.EndTime != auction.EndTime)
            {
                product.EndTime = auction.EndTime;
            }

            if (auction.ImageFile != null)
            {
                var result = _auctionService.SaveImage(auction.ImageFile);
                if (result.Item1 == 1)
                {
                    var oldImage = product.Picture;
                    product.Picture = result.Item2;
                     _auctionContext.SaveChanges();

                    var deleteResult = _auctionService.DeleteImage(oldImage);
                }
            }
            _auctionContext.SaveChanges();
            return RedirectToAction("Details", "User", new { id = product.Id });
            

        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Bid(int auctionItemId, decimal bidAmount)
        {
           
                var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
                var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked");
            }
            if (ModelState.IsValid)
                {
                    // Check if the user is already the highest bidder
                    //var currentBid = _auctionContext.Bids.Where(b => b.AuctionGood.Id == auctionItemId)?.Max(b => b.BidAmount)??0;
                    var currentBid = GetHighestBidAmount(auctionItemId);
                    if (bidAmount >= currentBid)
                    {
                        var item = _auctionContext.AuctionGoods.Where(b => b.Id == auctionItemId).FirstOrDefault();
                        // Create a new bid
                        var bid = new Biddings
                        {
                            AuctionGood = item,
                            BidAmount = bidAmount,
                            BidTime = DateTime.Now,
                            BidderId = currentUser,
                            Bidder = currentUser.Id,
                            goodid = item.Id,
                        };

                        // Save the bid to the database
                        _auctionContext.Bids.Add(bid);
                        await _auctionContext.SaveChangesAsync();

                        // Redirect to the auction page
                        return RedirectToAction("Details", new { id = auctionItemId });
                    }
                }

                return BadRequest();
           
        }


    }
}
