using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Agif_V2.Controllers
{
    public class EmiCalculatorController : Controller
    {
        private readonly IClaimCalculator _claimCalculator;

        public EmiCalculatorController(IClaimCalculator claimCalculator)
        {
            _claimCalculator = claimCalculator;
        }
        public IActionResult Calculator()
        {
            return View();
        }

        public IActionResult MaturityCalculator()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Calculate(int month, int year, int categoryValue)
        {
           var Investmentvalue =await _claimCalculator.CalculateTotalInvestment(month, year,categoryValue);
            return Json(new { success = true, message = "Calculation completed successfully.", value = Investmentvalue });
        }


    }
}
