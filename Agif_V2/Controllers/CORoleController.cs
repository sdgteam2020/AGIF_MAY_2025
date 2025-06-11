using DataAccessLayer;
using DataTransferObject.Identitytable;
using DataTransferObject.Request;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agif_V2.Controllers
{
    public class CORoleController : Controller
    {

        private readonly ApplicationDbContext _db;
        public CORoleController( ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult UserApplicationList()
        {
            return View();
        }
        public JsonResult GetAllUSerApplicationlist(int TypeId)
        {
            List<DTOCoDashBoard> list = new List<DTOCoDashBoard>();
            List<DTOCoDashBoard> data;
            int userId  = Convert.ToInt32(HttpContext.Session.GetString("UserGUID").ToString());
            int unitId = _db.UserMappings.FirstOrDefaultAsync(x => x.UserID == userId).Result.UnitId;
            if(TypeId == 1) // For CO
            {
                //data = _db.CoDashBoards.Where(x => x.UnitId == unitId && x.TypeId == TypeId).ToList();
            }
            else if(TypeId == 2) // For User
            {
                //data = _db.CoDashBoards.Where(x => x.UnitId == unitId && x.TypeId == TypeId).ToList();
            }
            else if(TypeId == 3) // For All
            {
                //data = _db.CoDashBoards.Where(x => x.UnitId == unitId).ToList();
            }
            else
            {
                return Json(new { message = "No Record Found" });
            }

            return Json("1");
        }
    }
}
