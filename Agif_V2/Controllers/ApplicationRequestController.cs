using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Mvc;

namespace Agif_V2.Controllers
{
    public class ApplicationRequestController : Controller
    {
        private readonly IApplication _application;
        public ApplicationRequestController(IApplication application)
        {
            this._application = application;
        }
        //public IActionResult UserApplications(int status)
        //{
        //    DTOGetApplRequest request = new DTOGetApplRequest
        //    {
        //        status = status
        //    };
        //    return View(request);
        //}
        public async Task<IActionResult> UserApplications(int status = 1)
        {
            try
            {
                DTOGetApplRequest request = new DTOGetApplRequest
                {
                    status = status
                };

                var applications = await _application.GetApplicationsAsync(request);

                ViewBag.CurrentStatus = status;

                return View();
            }
            catch (Exception ex)
            {
                // Log error and show error view
                ViewBag.ErrorMessage = "Error loading applications: " + ex.Message;
                return View(new List<DTOGetApplResponse>());
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUserApplications(int status)
        {
            try
            {
                DTOGetApplRequest request = new DTOGetApplRequest
                {
                    status = status
                };

                var applications = await _application.GetApplicationsAsync(request);

                return Json(new
                {
                    success = true,
                    data = applications,
                    count = applications.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
