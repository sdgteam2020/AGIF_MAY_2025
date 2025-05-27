using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Threading.Tasks;

namespace Agif_V2.Controllers
{
    public class OnlineApplicationController : Controller
    {
        private readonly IOnlineApplication _IonlineApplication1;
        private readonly IMasterOnlyTable _IMasterOnlyTable;
        public OnlineApplicationController(IOnlineApplication OnlineApplication, IMasterOnlyTable MasterOnlyTable)
        {
            _IonlineApplication1 = OnlineApplication;
            _IMasterOnlyTable = MasterOnlyTable;
        }

        public async Task<IActionResult> OnlineApplication()
        {
           
            return View();
        }
        public IActionResult SaveApplication(DTOOnlineApplicationRequest Data)
        {
            var validationContext = new ValidationContext(Data.onlineApplications);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(
                Data.onlineApplications,
                validationContext,
                validationResults,
                validateAllProperties: true);

            if (!isValid)
            {
                // validationResults contains errors
            }
            return View();  
        }
        public IActionResult LoanType()
        {
            return View();
        }
        public async Task <JsonResult> GetRetirementDate(int rankId,int Prefix)
        {
            var userType = await _IMasterOnlyTable.GetUserType(Prefix);
            var retAge = await _IMasterOnlyTable.GetRetirementAge(rankId);
            var retirementAge = retAge.FirstOrDefault()?.RetirementAge ?? 0;
            var userTypeId = userType.FirstOrDefault()?.UserType ?? 0;
            if(retirementAge > 0 && userTypeId != 0)
            {

                return Json(new { retirementAge = retirementAge, userTypeId = userTypeId });

            }
            else
            {
                return Json("0");
            }
        }
        public async Task<JsonResult> GetPCDA_PAO(int regt)
        {
            var pcda = await _IMasterOnlyTable.GetPCDA_PAO(regt);
            var pcdaPao = pcda.FirstOrDefault()?.Pcda_Pao ?? string.Empty;
            if(!string.IsNullOrEmpty(pcdaPao))
            {
                return Json(new { pcdaPao = pcdaPao });
            }
            else
            {
                return Json("0");
            }
        }
    }
}
