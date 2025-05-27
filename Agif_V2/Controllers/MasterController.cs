using DataTransferObject.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Interfaces;
using DataTransferObject.Response;
using DataTransferObject.Helpers;
namespace Agif_V2.Controllers
{
    public class MasterController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public MasterController(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;


        }
        public IActionResult Index()
        {
            return View();
        }
        #region Master Table 
        [AllowAnonymous]
        public async Task<IActionResult> GetAllMMaster(DTOMasterRequest data)
        {
            try
            {
                var result = await _unitOfWork.GetAllMMaster(data);
                return Json(result);
            }
            catch (Exception)
            {
                return Json(Constants.InternalServerError);
            }
        }

        #endregion
    }
}
