using Auction.Data;
using Auction.Models;
using Auction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Auction.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationDbContext _auctionContext;
        private readonly IFileService _fileService;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(ApplicationDbContext auctionContext, UserManager<ApplicationUser> userManager, IFileService fileService , SignInManager<ApplicationUser> signInManager)
        {
            _auctionContext = auctionContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _fileService = fileService;
        }
        public async Task<int> GetAuctionsWithHighestBid(string userId)
        {
            DateTime currentDateTime = DateTime.Now;
            var auctionsWithHighestBid = await _auctionContext.AuctionGoods
                .Where(ag => ag.Seller.Id != userId && ag.EndTime <= currentDateTime && ag.Seller.userdef == "active")
                .OrderByDescending(ag => ag.Bids.Max(b => b.BidAmount))
                .Include(ag => ag.Bids)
                .ToListAsync();
            var count = auctionsWithHighestBid.Count;
            return count;

        }

        public async Task<IActionResult> Index()
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked","Auctions");
            }
            var bid = _auctionContext.Bids.Where(b => b.Bidder == currentUser.Id).ToList();
            var bids = bid.Count;
            ViewData["mybids"] = bids;
            var myauctions = _auctionContext.AuctionGoods.Where(b => b.Seller.Id == currentUser.Id).ToList().Count;
            ViewData["myauctions"] = myauctions;
            var wins = await GetAuctionsWithHighestBid(currentUser.Id);
            ViewData["mywins"] = wins;
            var bidhistory = _auctionContext.Bids.Where(b => b.Bidder == currentUser.Id).OrderByDescending(b=>b.BidTime).ToList();
            return View(bidhistory);
        }
      
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked", "Auctions");
            }
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> Password()
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked", "Auctions");
            }
            return View(new PasswordViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordChange(PasswordViewModel  model)
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked", "Auctions");
            }
            var user = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                if (model.NewPassowrd != model.ConfirmPassword)
                {
                    TempData["Message"] = "New Passwords Not Matched.";
                    return RedirectToAction("Password");

                }
                else
                {
                    if(await _userManager.CheckPasswordAsync(user, model.OldPassword))
                    {
                        await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassowrd);
                        TempData["Message"] = "Password changed successfully.";
                        return RedirectToAction("Password");
                    }
                    else
                    {
                        TempData["Message"] = "Current Password Don't Matches";
                        return RedirectToAction("Password");
                    }
 
                }
              
            }
            TempData["Message"] = "Password Not changed successfully.";

            return RedirectToAction("Password");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserViewModel usermodel)
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked", "Auctions");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
               
                return RedirectToAction("Index");
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (usermodel.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, usermodel.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return RedirectToAction("Profile");
                }
            }
            if (usermodel.Name != user.Name)
            {
                user.Name = usermodel.Name;
                await _userManager.UpdateAsync(user);
            }

            if (usermodel.ImageFile != null)
            {
                var result = _fileService.SaveImage(usermodel.ImageFile);
                if (result.Item1 == 1)
                {
                    var oldImage = user.ProfilePicture;
                    user.ProfilePicture = result.Item2;
                    await _userManager.UpdateAsync(user);
                    var deleteResult = _fileService.DeleteImage(oldImage);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Profile");
        }
            public async Task<IActionResult> Winningbids()
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked", "Auctions");
            }
            DateTime currentDateTime = DateTime.Now;
            var data = await _auctionContext.AuctionGoods
                .Where(ag => ag.Seller.Id != currentUser.Id && ag.EndTime <= currentDateTime)
                .OrderByDescending(ag => ag.Bids.Max(b => b.BidAmount))
                .Include(ag => ag.Bids)
                .ToListAsync();
            return View(data);
        }
        public async Task<IActionResult> Myauctions()
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked", "Auctions");
            }
            var auctions = _auctionContext.AuctionGoods.Where(b=>b.Seller.Id ==currentUser.Id).OrderBy(b=>b.StartTime).ToList();
            return View(auctions);
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Allusers()
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked", "Auctions");
            }
            var users = _auctionContext.Users.Where(a => a.Id != currentUser.Id);
            return View(users);
        }
        public async Task<IActionResult> Details(int? id)
        {
            var hasExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).Any();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.userdef != "active")
            {
                return RedirectToAction("Blocked", "Auctions");
            }
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
           
                if (item.Seller.Id == currentUser.Id)
                {
                return View(item);
               
                }

            return RedirectToAction("Index");
  
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> ChangeStatus(string? id)
        {
            var user = _auctionContext.Users.Where(b=>b.Id == id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Allusers");
            }
            user.userdef = "block";
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Allusers");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unblock(string? id)
        {
            var user = _auctionContext.Users.Where(b => b.Id == id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Allusers");
            }
            user.userdef = "active";
            await _userManager.UpdateAsync(user);
            return RedirectToAction("Allusers");
        }
        
    }
}
