using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

namespace Agif_V2.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public DefaultController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult InsuranceCover()
        {
            return View();
        }
        public IActionResult AdditionalInsuranceCovertoArmyAviationOfficers()
        {
            return View();
        }
        public IActionResult DisabilityBenefits()
        {
            return View();
        }
        public IActionResult ExGratiaDisabilityAllowance()
        {
            return View();
        }
        public IActionResult MaturityBenefits()
        {
            return View();
        }

        public IActionResult FinalWithdrawalfromMaturityBenefit()
        {
            return View();
        }

        public IActionResult SustenanceAllowancetoDifferentlyAbledChildren()
        {
            return View();
        }
        public IActionResult ExtendedInsuranceScheme()
        {
            return View();
        }
        public IActionResult SocialSecurityDepositsScheme()
        {
            return View();
        }
        public IActionResult HbaApplication()
        {
            return View();
        }
    }
}
