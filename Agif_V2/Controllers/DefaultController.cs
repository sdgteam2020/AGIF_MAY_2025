using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

namespace Agif_V2.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IDefault _default;
        public DefaultController(IDefault _default)
        {
            this._default = _default;
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
        public IActionResult COContactUs()
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
        public IActionResult CheckApplicationStatus()
        {
            return View();
        }
        public async Task<IActionResult> SearchByArmyNo(string armyNo)
        {
            var data = await _default.GetUserApplicationStatusByArmyNo(armyNo);
            return Json(data);
        }


        public async Task<IActionResult> ClaimSearchByArmyNo(string armyNo)
        {
            var data = await _default.GetClaimUserApplicationStatusByArmyNo(armyNo);
            return Json(data);
        }

        public async Task<IActionResult> GetTimeline(int ApplicationId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await _default.GetTimeLine(ApplicationId);
            return Json(data);
        }

        public async Task<IActionResult> GetClaimTimeline(int ApplicationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await _default.GetClaimTimeLine(ApplicationId);
            return Json(data);
        }


    }
}
