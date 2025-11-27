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
        private readonly IHome home;
        
        public HomeController(IHome home)
        {
            this.home = home;
        }

        public async Task<IActionResult> Index()
        {
            var sessionUser = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (sessionUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if(sessionUser.Role == "LoanAdmin")
            {
                var userCounts = await home.GetUserCount();
                var metrics = new Dictionary<string, int>
                {
                    ["ActiveUsers"] = userCounts.FirstOrDefault(x => x.Status == "Active")?.Count ?? 0,
                    ["InactiveUsers"] = userCounts.FirstOrDefault(x => x.Status == "Inactive")?.Count ?? 0
                };

                ViewBag.Metrics = metrics;
            }

            else if (sessionUser.Role == "ClaimAdmin")
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
        [Authorize(Roles = "Admin")]
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
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request." });
            }
            try
            {
                var queryableData = await home.GetApprovedLogs();
                var totalRecords = queryableData.Count;
                var query = queryableData.AsQueryable();

                query = AdminApplySearchFilter(query, request.searchValue);

                var filteredRecords = query.Count();

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
        private IQueryable<DTOApprovedLogs> AdminApplySearchFilter(IQueryable<DTOApprovedLogs> query, string? searchValue)
        {
            if (string.IsNullOrEmpty(searchValue)) return query;

            string lowerSearchValue = searchValue.ToLower();
            return query.Where(x =>
                
                (x.DomainId ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
                (x.Name ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
                (x.UpdatedOn.ToString() ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase) ||
                (x.CoDomainId ?? string.Empty).Contains(lowerSearchValue, StringComparison.CurrentCultureIgnoreCase)
            );
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


        [HttpPost]
        public IActionResult AnalyticsDashBoard(int id)
        {
            SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
            if (dTOTempSession == null || dTOTempSession.ProfileId <= 0)
            {
                return Unauthorized("Session expired or invalid user session.");
            }
            ViewBag.ArmyNo = dTOTempSession.ArmyNo;
            ViewBag.AdminTypeId = id;
            return View(dTOTempSession);
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicationAnalytics(int year)
        {
            try
            {
                var analyticsData = await home.GetTotalMonthlyApplications(year);
                return Json(new { success = true, data = analyticsData });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = "An error occurred while fetching analytics data: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetClaimApplicationAnalytics(int year)
        {
            try
            {
                var analyticsData = await home.GetTotalClaimMonthlyApplications(year);
                return Json(new { success = true, data = analyticsData });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = "An error occurred while fetching analytics data: " + ex.Message });
            }
        }
    }
}
