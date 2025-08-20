using System.Diagnostics;
using Agif_V2.Models;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Agif_V2.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IHome home;
        
        public HomeController(IHome home)
        {
            this.home = home;
            //_logger = logger;
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

        public  IActionResult LogViewer()
        {
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.ProfileId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }
            var res = home.GetApprovedLogs();
            ViewBag.ArmyNo = dTOTempSession.ArmyNo;
            return View(dTOTempSession);
        }

        public async Task<IActionResult> GetApprovedLogs(DTODataTableRequest request)
        {
            try
            {
                var queryableData = await home.GetApprovedLogs();
                var totalRecords = queryableData.Count;
                var query = queryableData.AsQueryable();

                //// Apply search filter
                //query = AdminApplySearchFilter(query, request.searchValue);

                var filteredRecords = query.Count();

                //// Apply sorting
                //query = AdminApplySorting(query, request.sortColumn, request.sortDirection);

                // Paginate the result
                var paginatedData = query.Skip(request.Start).Take(request.Length).ToList();

                var responseData = new DTODataTablesResponse<DTOApprovedLogs>
                {
                    draw = request.Draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = filteredRecords,
                    data = paginatedData
                };

                return Json(responseData);
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { error = "An error occurred while loading data: " + ex.Message });
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
