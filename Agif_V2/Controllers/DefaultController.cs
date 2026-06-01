using Agif_V2.Helpers;
using DataAccessLayer.Interfaces;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Agif_V2.Controllers
{

    [AutoValidateAntiforgeryToken]
    public class DefaultController : Controller
    {
        public const string SessionKeySalt = "_Salt";
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
        [HttpGet]
        public IActionResult CheckApplicationStatus()
        {
            if (Request.Query.Count > 0)
            {
                return BadRequest("Invalid request");
            }
            string dd = AESEncrytDecry.GetKey(32);
            HttpContext.Session.SetString(SessionKeySalt, dd);
            ViewBag.hdns = dd;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckApplicationStatusPost([FromForm] string EncryptedData)
        {
            return View("CheckApplicationStatus");
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Ensure this matches your JS header
        public async Task<IActionResult> SearchByArmyNo([FromForm] string EncryptedData)
        {
            // 1. Check if payload was received
            if (string.IsNullOrEmpty(EncryptedData))
            {
                return Json(new { success = false, message = "Form data is missing or corrupted." });
            }

            // 2. Retrieve the decryption key from the session
            var keyBase64 = HttpContext.Session.GetString(SessionKeySalt);
            if (string.IsNullOrEmpty(keyBase64))
            {
                return Json(new { success = false, message = "Session expired. Please refresh the page." });
            }

            // 3. Decrypt the payload
            string decryptedJson;
            try
            {
                decryptedJson = AESEncrytDecry.DecryptAES(EncryptedData, keyBase64);
                decryptedJson = decryptedJson.Trim('\0', ' '); // Clean up AES padding
            }
            catch (CryptographicException)
            {
                return Json(new { success = false, message = "Invalid or tampered data." });
            }

            // 4. Deserialize the JSON string back into C# variables
            DTOApplicationSearch searchParams;
            try
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                searchParams = System.Text.Json.JsonSerializer.Deserialize<DTOApplicationSearch>(decryptedJson, options);
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Security error: Failed to process the payload." });
            }

            // 5. Apply your existing business validation rules
            if (string.IsNullOrWhiteSpace(searchParams.ArmyNo))
            {
                return Json(new { success = false, message = "Army number is required" });
            }

            if (string.IsNullOrWhiteSpace(searchParams.AadharNo))
            {
                return Json(new { success = false, message = "Aadhar number is required" });
            }

            if (!Regex.IsMatch(searchParams.ArmyNo, @"^[a-zA-Z0-9]{1,20}$"))
            {
                return Json(new { success = false, message = "Invalid army number format" });
            }

            // 6. Execute your existing database query
            var data = await _default.GetUserApplicationStatusByArmyNo(searchParams.ArmyNo, searchParams.AadharNo);

            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> ClaimSearchByArmyNo([FromForm] string armyNo, string aadharNo)
        {
            if (string.IsNullOrWhiteSpace(armyNo))
            {
                return Json(new { success = false, message = "Army number is required" });
            }
            if (string.IsNullOrWhiteSpace(aadharNo))
            {
                return Json(new { success = false, message = "Aadhar number is required" });
            }

            if (!Regex.IsMatch(armyNo, @"^[a-zA-Z0-9]{1,20}$"))
            {
                return Json(new { success = false, message = "Invalid Aadhar number format" });
            }
            var data = await _default.GetClaimUserApplicationStatusByArmyNo(armyNo,aadharNo);
            return Json(data);
        }
        [HttpPost]
        public async Task<IActionResult> GetTimeline(int ApplicationId)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var data = await _default.GetTimeLine(ApplicationId);
            return Json(data);
        }
        [HttpPost]
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

        private string GetClientIp()
        {
            var forwardedHeader = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                return forwardedHeader.Split(',')[0].Trim();
            }

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> DownloadApplication(int id)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            string ipAddress = GetClientIp();

            DTOExportRequest dTOExport = new DTOExportRequest { Id = new List<int> { id } };
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

            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempPdf");
            Directory.CreateDirectory(tempFolder);

            string tempFilePath = Path.Combine(tempFolder, originalFileName);
            System.IO.File.Copy(originalFilePath, tempFilePath, overwrite: true);

            _watermark.AddAnnotationAfterDigitalSign(ipAddress, tempFilePath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);

            System.IO.File.Delete(tempFilePath);

            return File(fileBytes, "application/pdf", originalFileName);
            
        }

        [HttpPost]
        public async Task<IActionResult> DownloadClaimApplication(int id)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid request." });
            }

            string ipAddress = GetClientIp();

            DTOExportRequest dTOExport = new DTOExportRequest { Id = new List<int> { id } };
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

            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempPdf");
            Directory.CreateDirectory(tempFolder);

            string tempFilePath = Path.Combine(tempFolder, originalFileName);
            System.IO.File.Copy(originalFilePath, tempFilePath, overwrite: true);

            _watermark.AddAnnotationAfterDigitalSign(ipAddress, tempFilePath);

            byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);

            System.IO.File.Delete(tempFilePath);

            return File(fileBytes, "application/pdf", originalFileName);
        }
    }
}
