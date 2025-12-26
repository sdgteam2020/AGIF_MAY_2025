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

        
        //public async Task<JsonResult> Calculate(int month, int year, int categoryValue)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Json("Invalid request");
        //    }   

        //    var result = await _claimCalculator.CalculateTotalInvestment(month, year,categoryValue);
        //    return Json(new{success = true,CurrentBalance = result.currentBalance,BalCount = result.balCount,SaveEL = result.saveEL});
        //}
        [HttpPost]
        public async Task<IActionResult> Calculate(int month, int year, int categoryValue, int? commissionMonth, int? commissionYear)
        {
            try
            {
                var result = await _claimCalculator.CalculateTotalInvestment(month, year, categoryValue, commissionMonth, commissionYear);

                return Json(new
                {
                    success = true,
                    currentBalance = result.currentBalance,
                    balCount = result.balCount,
                    saveEL = result.saveEL
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
