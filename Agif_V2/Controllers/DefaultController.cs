using Agif_V2.Helpers;
using DataAccessLayer.Interfaces;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;

namespace Agif_V2.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IDefault _default;
        private readonly IOnlineApplication _onlineApplication;
        private readonly IClaimOnlineApplication _IClaimonlineApplication;
        private readonly Watermark _watermark;
        public DefaultController(IDefault _default, IOnlineApplication _onlineApplication, Watermark _watermark, IClaimOnlineApplication iClaimonlineApplication)
        {
            this._default = _default;
            this._onlineApplication = _onlineApplication;
            this._watermark = _watermark;
            _IClaimonlineApplication = iClaimonlineApplication;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Index1()
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
        public IActionResult Message()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        public async Task<IActionResult> DownloadApplication([FromQuery] List<int> id)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IpAddress";

            DTOExportRequest dTOExport = new DTOExportRequest { Id = id };
            var ret = await _onlineApplication.GetApplicationDetailsForExport(dTOExport);

            var firstRecord = ret.OnlineApplicationResponse.FirstOrDefault();
            if (firstRecord == null)
            {
                return Json(new { success = false, message = "No record found." });
            }

            string armyNo = firstRecord.Number ?? "UnknownArmyNo";
            int applicationId = firstRecord.ApplicationId;
            string originalFileName = $"App{applicationId}{armyNo}.pdf";

            string originalFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "MergePdf",
                originalFileName
            );

            if (!System.IO.File.Exists(originalFilePath))
            {
                return Json(new { success = false, message = "Merged PDF not found." });
            }

            // STEP 1: Create a temp folder inside /wwwroot/
            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempPdf");
            Directory.CreateDirectory(tempFolder);

            // STEP 2: Copy original to temp file
            string tempFilePath = Path.Combine(tempFolder, originalFileName);
            System.IO.File.Copy(originalFilePath, tempFilePath, overwrite: true);

            // STEP 3: Apply watermark on temp file
            // (This modifies ONLY the copied file, NOT the main one)
            _watermark.AddAnnotationAfterDigitalSign(ipAddress, tempFilePath);

            // STEP 4: Read the watermarked temp file for download
            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);

            // STEP 5: Delete temp file after reading (optional but recommended)
            System.IO.File.Delete(tempFilePath);

            // STEP 6: Return PDF file for download
            return File(fileBytes, "application/pdf", originalFileName);
        }
        public async Task<IActionResult> DownloadClaimApplication([FromQuery] List<int> id)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IpAddress";

            DTOExportRequest dTOExport = new DTOExportRequest { Id = id };
            var ret = await _IClaimonlineApplication.GetApplicationDetailsForExport(dTOExport);

            var firstRecord = ret.OnlineApplicationResponse.FirstOrDefault();
            if (firstRecord == null)
            {
                return Json(new { success = false, message = "No record found." });
            }

            string armyNo = firstRecord.Number ?? "UnknownArmyNo";
            int applicationId = firstRecord.ApplicationId;
            string originalFileName = $"App{applicationId}{armyNo}.pdf";

            string originalFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "ClaimMergePdf",
                originalFileName
            );

            if (!System.IO.File.Exists(originalFilePath))
            {
                return Json(new { success = false, message = "Merged PDF not found." });
            }

            // STEP 1: Create a temp folder inside /wwwroot/
            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempPdf");
            Directory.CreateDirectory(tempFolder);

            // STEP 2: Copy original to temp file
            string tempFilePath = Path.Combine(tempFolder, originalFileName);
            System.IO.File.Copy(originalFilePath, tempFilePath, overwrite: true);

            // STEP 3: Apply watermark on temp file
            // (This modifies ONLY the copied file, NOT the main one)
            _watermark.AddAnnotationAfterDigitalSign(ipAddress, tempFilePath);

            // STEP 4: Read the watermarked temp file for download
            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);

            // STEP 5: Delete temp file after reading (optional but recommended)
            System.IO.File.Delete(tempFilePath);

            // STEP 6: Return PDF file for download
            return File(fileBytes, "application/pdf", originalFileName);
        }
    }
}
