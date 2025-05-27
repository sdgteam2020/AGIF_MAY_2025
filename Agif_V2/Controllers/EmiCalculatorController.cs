using Microsoft.AspNetCore.Mvc;

namespace Agif_V2.Controllers
{
    public class EmiCalculatorController : Controller
    {
        public IActionResult Calculator()
        {
            return View();
        }
    }
}
