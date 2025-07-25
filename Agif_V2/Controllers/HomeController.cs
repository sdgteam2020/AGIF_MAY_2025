using System.Diagnostics;
using Agif_V2.Models;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Agif_V2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHome home;
        
        public HomeController(ILogger<HomeController> logger, IHome home)
        {
            this.home = home;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var sessionUser = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (sessionUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if(sessionUser.Role == "Admin")
            {
                var userCounts = await home.GetUserCount();
                var metrics = new Dictionary<string, int>
                {
                    ["ActiveUsers"] = userCounts.FirstOrDefault(x => x.Status == "Active")?.Count ?? 0,
                    ["InactiveUsers"] = userCounts.FirstOrDefault(x => x.Status == "Inactive")?.Count ?? 0
                };

                ViewBag.Metrics = metrics;
            }

            else if (sessionUser.Role == "MaturityAdmin")
            {
                var userCounts = await home.GetUserCount();
                var metrics = new Dictionary<string, int>
                {
                    ["ActiveUsers"] = userCounts.FirstOrDefault(x => x.Status == "Active")?.Count ?? 0,
                    ["InactiveUsers"] = userCounts.FirstOrDefault(x => x.Status == "Inactive")?.Count ?? 0
                };

                ViewBag.Metrics = metrics;
            }
            else
            {
                var applCount = await home.GetApplicationCount(sessionUser.UserId);
                var metrics = new Dictionary<string, int>
                {
                    ["PendingApplications"] = applCount.FirstOrDefault(x => x.Status == "Pending")?.Count ?? 0,
                    ["ApprovedApplications"] = applCount.FirstOrDefault(x => x.Status == "Approved")?.Count ?? 0,
                    ["RejectedApplications"] = applCount.FirstOrDefault(x => x.Status == "Rejected")?.Count ?? 0,
                };

                ViewBag.Metrics = metrics;
            }
            return View(sessionUser);
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
