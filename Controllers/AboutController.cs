using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            return View();
        }
    }
}
