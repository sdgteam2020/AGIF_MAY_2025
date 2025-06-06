using DataTransferObject.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Agif_V2.Controllers
{
    public class ApplicationController : Controller
    {
        public IActionResult UserApplications(int status)
        {
            return View();
        }
        public JsonResult GetUserApplications(int status)
        {
            return Json("2");
        }
        
    }
}
