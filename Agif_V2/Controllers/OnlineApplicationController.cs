using Agif_V2.Helpers;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Agif_V2.Controllers
{
    public class OnlineApplicationController : Controller
    {
        public const string SessionKeySalt = "_Salt";
        private readonly IOnlineApplication _IonlineApplication1;
        private readonly IMasterOnlyTable _IMasterOnlyTable;
        private readonly PdfGenerator _pdfGenerator;
        private readonly MergePdf _mergePdf;
        private readonly IWebHostEnvironment _env;
        private readonly ICar _car;
        private readonly IHba _Hba;
        private readonly IPca _Pca;
        private readonly FileUtility _fileUtility;
        private readonly IAddress _address;
        private readonly IAccount _account;
        private readonly IModelStateLogger _modelStateLogger;
        private readonly ModelValidations _modelValidations;
        public OnlineApplicationController(IOnlineApplication OnlineApplication, IMasterOnlyTable MasterOnlyTable, ICar _car, IHba _Hba, IPca _Pca, PdfGenerator pdfGenerator, IWebHostEnvironment env, MergePdf mergePdf, IAddress address, IAccount account, FileUtility fileUtility, IModelStateLogger modelStateLogger, ModelValidations modelValidations)
        {
            _IonlineApplication1 = OnlineApplication;
            _IMasterOnlyTable = MasterOnlyTable;
            this._car = _car;
            this._Hba = _Hba;
            this._Pca = _Pca;
            _pdfGenerator = pdfGenerator;
            _env = env;
            _mergePdf = mergePdf;
            _fileUtility = fileUtility;
            _address = address;
            _account = account;
            _modelStateLogger = modelStateLogger;
            _modelValidations = modelValidations;
        }
        [HttpGet]
        public IActionResult OnlineApplication()
        {
            string dd = AESEncrytDecry.GetSalt();
            HttpContext.Session.SetString(SessionKeySalt, dd);
            ViewBag.hiddenSalt = dd;
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }

            var loanType = TempData["LoanType"] as string;
            var applicantCategory = TempData["ApplicantCategory"] as string;

            int id = TempData["RedirectapplicationId"] is int applicationId ? applicationId : 0;
            TempData["loantypeNew"] = loanType ?? string.Empty;
            TempData["applicantcategoryNew"] = applicantCategory ?? string.Empty;

            var response = new DTOCommonOnlineApplicationResponse();
            response = null;

            DTOOnlineApplication DTOOnlineapplication = new DTOOnlineApplication();

            if (id != 0)
            {
                response = _IonlineApplication1.GetApplicationAndApplicantType(id);
            }

            if (response != null)
            {
                DTOOnlineapplication.loantype = response.OnlineApplicationResponse.ApplicationType.ToString();
                DTOOnlineapplication.applicantCategory = response.OnlineApplicationResponse.ApplicantType.ToString();
            }


            TempData.Keep("LoanType");
            TempData.Keep("ApplicantCategory");



            return View(DTOOnlineapplication);
        }
        public IActionResult LoanType()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetRetirementDate(int rankId, int Prefix, int regtId)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }
            var prefixRankRetirementMap = new Dictionary<(int prefix, int rank), int>
            {
                { (11, 21), 57 },
                { (11, 22), 57 },
                { (11, 23), 57 },
                { (11, 24), 57 },
                { (11, 29), 57 },
                { (11, 26), 59 },
                { (11, 27), 60 },
                { (11, 28), 61 },

                { (3, 21), 57 },
                { (3, 22), 57 },
                { (3, 23), 57 },
                { (3, 24), 57 },
                { (3, 29), 57 },
                { (3, 26), 58 },
                { (3, 27), 59 },
                { (3, 28), 60 },
            };

            if (prefixRankRetirementMap.TryGetValue((Prefix, rankId), out int retirementAge))
            {
                return Json(new { retirementAge, userTypeId = 1 });
            }

            var userType = await _IMasterOnlyTable.GetUserType(Prefix);
            var retAge = await _IMasterOnlyTable.GetRetirementAge(rankId, regtId);

            retirementAge = retAge.FirstOrDefault()?.RetirementAge ?? 0;
            var userTypeId = userType.FirstOrDefault()?.UserType ?? 0;

            return (retirementAge > 0 && userTypeId != 0)
                ? Json(new { retirementAge, userTypeId })
                : Json("0");
        }
        public async Task<JsonResult> GetPCDA_PAO(int regt)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }
            var pcda = await _IMasterOnlyTable.GetPCDA_PAO(regt);
            var pcdaPao = pcda.FirstOrDefault()?.Pcda_Pao ?? string.Empty;
            if (!string.IsNullOrEmpty(pcdaPao))
            {
                return Json(new { pcdaPao = pcdaPao });
            }
            else
            {
                return Json("0");
            }
        }



        [HttpPost]
        public async Task<JsonResult> CheckExistUser([FromBody] CommonParameters model)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }

            var existingUser = await _IonlineApplication1.GetApplicationDetailsByArmyNo(model.armyNumber, model.Prefix, model.Suffix, model.appType);

            if (existingUser != null)
            {
                return Json(new { exists = true });
            }
            else
            {
                return Json(new { exists = false });
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeleteExistingLoan([FromBody] CommonParameters model)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }

            bool result = await _IonlineApplication1.DeleteExistingLoan(model.armyNumber, model.Prefix, model.Suffix, model.appType);

            if (result == true)
            {
                return Json(new { exists = true });
            }
            else
            {
                return Json(new { exists = false });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Redirection(string loanType, string applicantCategory)
        {

            TempData["LoanType"] = loanType;
            TempData["ApplicantCategory"] = applicantCategory;

            return RedirectToAction("OnlineApplication");
        }

        public async Task<JsonResult> CheckForCoRegister(string ArmyNo)
        {
            return Json(await _IonlineApplication1.CheckForCoRegister(ArmyNo));
        }

        public async Task<JsonResult> CheckIsUnitRegister(string ArmyNo)
        {
            return Json(await _IonlineApplication1.CheckIsUnitRegister(ArmyNo));
        }

        public async Task<JsonResult> CheckIsCoRegister(int UnitId)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }
            return Json(await _IonlineApplication1.CheckIsCoRegister(UnitId));
        }

        [HttpPost]
        public async Task<IActionResult> OnlineApplication([FromForm] string EncryptedData)
        {
            //// 1. Validate that we actually received the encrypted payload
            string dd = HttpContext.Session.GetString(SessionKeySalt);

            if (string.IsNullOrEmpty(dd))
            {
                dd = AESEncrytDecry.GetSalt();
                HttpContext.Session.SetString(SessionKeySalt, dd);
            }

            ViewBag.hiddenSalt = dd;

            if (string.IsNullOrEmpty(EncryptedData))
            {
                ModelState.AddModelError("", "Form data is missing or corrupted.");
                return View("OnlineApplication", new DTOOnlineApplication());
            }

            DTOOnlineApplication model;

            try
            {
                string secretKey = HttpContext.Session.GetString(SessionKeySalt);

                string decryptedJson = AESEncrytDecry.DecryptAES(EncryptedData, secretKey);

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,

                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };
                options.Converters.Add(new Agif_V2.Helpers.FlexibleDateTimeConverter());
                options.Converters.Add(new Agif_V2.Helpers.FlexibleDecimalConverter()); // <-- Added this
                model = System.Text.Json.JsonSerializer.Deserialize<DTOOnlineApplication>(decryptedJson, options);

                TryValidateModel(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Security error: Failed to process the application payload.");
                return View("OnlineApplication", new DTOOnlineApplication());
            }

            string formType = GetFormType(model);
            if (formType == null)
            {
                ModelState.AddModelError("", "Please select an application type.");
            }

            

            ValidateModel(model.AddressDetails, "AddressDetails");
            ValidateModel(model.AccountDetails, "AccountDetails");
            ValidateModel(model.CommonData, "CommonData");

            

            switch (formType)
            {
                case "CA":
                    ValidateModel(model.CarApplication, "CarApplication");
                    break;
                case "PCA":
                    ValidateModel(model.PCAApplication, "PCAApplication");
                    break;
                case "HBA":
                    ValidateModel(model.HBAApplication, "HBAApplication");
                    break;
            }

            if (model.applicantCategory == "3") // OR
            {
                ModelState.Remove("CommonData.OldArmyPrefix");
                ModelState.Remove("CommonData.OldNumber");
                ModelState.Remove("CommonData.OldSuffix");
            }
          
            if (!_modelValidations.IsValidEmailDomain(model.CommonData.EmailDomain))
            {
                ModelState.AddModelError("CommonData.EmailDomain","Invalid email domain selected.");
            }
            var expectedSuffix = _modelValidations.CalculateSuffix(model.CommonData.Number);

            if (!string.Equals(expectedSuffix,model.CommonData.Suffix,StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("CommonData.Suffix","Invalid Army Number or Suffix.");
            }


            if (!ModelState.IsValid)
            {
                await _modelStateLogger.LogModelStateError(ModelState, HttpContext);
                return View("OnlineApplication", model);
            }

            CommonDataModel common = new CommonDataModel();
            try
            {
                if (model.CommonData != null)
                {
                    string? ip = GetClientIp();
                    model.CommonData.ApplicationType = int.Parse(model.loantype);
                    model.CommonData.ApplicantType = int.Parse(model.applicantCategory);
                    model.CommonData.IOArmyNo = string.IsNullOrEmpty(model.COArmyNo) ? "" : model.COArmyNo;
                    model.CommonData.ipAddress = ip;
                    if (model.CommonData.ApplicantType == 3)
                    {
                        model.CommonData.OldArmyPrefix = 0;
                        model.CommonData.OldNumber = string.Empty;   // or string.Empty if non-nullable
                        model.CommonData.OldSuffix = string.Empty;   // or string.Empty if non-nullable
                    }
                    if (string.IsNullOrEmpty(model.CommonData.pcda_AcctNo))
                    {
                        model.CommonData.pcda_AcctNo = string.Empty;
                    }
                    if (string.IsNullOrEmpty(model.CommonData.pcda_pao))
                    {
                        model.CommonData.pcda_pao = string.Empty;
                    }
                    common = await _IonlineApplication1.AddWithReturn(model.CommonData);
                }

                if (model.AddressDetails != null)
                {
                    model.AddressDetails.ApplicationId = common.ApplicationId;
                    await _address.Add(model.AddressDetails);
                }

                if (model.AccountDetails != null)
                {
                    model.AccountDetails.ApplicationId = common.ApplicationId;
                    await _account.Add(model.AccountDetails);
                }

                var formModelMap = new Dictionary<string, object>
                 {
                     { "HBA", model.HBAApplication },
                     { "CA", model.CarApplication },
                     { "PCA", model.PCAApplication }
                 };

                if (formModelMap.TryGetValue(formType, out var formModel) && formModel != null)
                {
                    dynamic appModel = formModel;
                    appModel.ApplicationId = common.ApplicationId;

                    switch (formType)
                    {
                        case "HBA":
                            await _Hba.Add(appModel);
                            break;
                        case "CA":
                            await _car.Add(appModel);
                            break;
                        case "PCA":
                            await _Pca.Add(appModel);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Security error: Failed to Process The Application.");
                return View("OnlineApplication", model);
            }

            TempData["applicationId"] = common.ApplicationId;
            TempData["Message"] = "Your application has been saved successfully. Please upload the required document to proceed.";
            TempData["COArmyNumber"] = model.COArmyNo;

            return RedirectToAction("Upload", "Upload");
        }

        private string? GetFormType(DTOOnlineApplication model)
        {
            if (model.CarApplication != null) return "CA";
            if (model.PCAApplication != null) return "PCA";
            if (model.HBAApplication != null) return "HBA";
            return null;
        }

        private void ValidateModel(object model, string prefix)
        {
            if (model == null) return;

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, true))
            {
                foreach (var result in results)
                {
                    string? propertyName = result.MemberNames?.FirstOrDefault();
                    string errorKey = string.IsNullOrEmpty(propertyName) ? prefix : $"{prefix}.{propertyName}";
                    ModelState.AddModelError(errorKey, result.ErrorMessage);
                }
            }
        }

        public async Task<JsonResult> MergePdf(int applicationId, bool isRejected, bool isApproved)
        {


            try
            {
                string ip = GetClientIp();
                var userData = await _IonlineApplication1.GetApplicationDetails(applicationId);
                if (userData == null) return JsonError("Application not found.");

                string applicationTypeName = GetApplicationTypeName(userData);
                if (string.IsNullOrEmpty(applicationTypeName)) return JsonError("Application type is not specified.");

                string armyNo = userData.OnlineApplicationResponse.Number;
                if (string.IsNullOrEmpty(armyNo)) return JsonError("Army number is not specified.");

                string applicationIdStr = applicationId.ToString();
                string folderPath = applicationTypeName + armyNo + "_" + applicationIdStr;
                string sourceFolderPath = Path.Combine(_env.WebRootPath, "TempUploads", folderPath);
                if (!Directory.Exists(sourceFolderPath))
                    return JsonError("Source folder not found");

                string[] pdfFiles = Directory.GetFiles(sourceFolderPath, "*.pdf");
                if (pdfFiles.Length == 0) return JsonError("No PDF files found in the specified folder.");

                pdfFiles = await GeneratePdfIfNeeded(applicationId, sourceFolderPath, folderPath, isRejected, isApproved, pdfFiles, ip);

                string mergedPdfPath = PrepareMergedPdfPath(applicationId, armyNo);
                ViewBag.MergedPdfPath = mergedPdfPath;

                bool mergeResult = await _mergePdf.MergePdfFiles(pdfFiles, mergedPdfPath);



                if (!mergeResult) return JsonError("Failed to merge PDF files.");

                string relativePath = mergedPdfPath.Replace(_env.WebRootPath, "").Replace("\\", "/");
                await _IonlineApplication1.UpdateMergePdfStatus(applicationId, true);

                return Json(new
                {
                    success = true,
                    message = "PDFs merged successfully.",
                    mergedFilePath = relativePath,
                    fullPath = mergedPdfPath,
                    totalFiles = pdfFiles.Length
                });
            }
            catch (Exception ex)
            {
                return JsonError($"Error occurred while merging PDFs");
            }
        }

        #region Helper Methods
        private string GetClientIp()
        {
            var forwardedHeader = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrEmpty(forwardedHeader))
            {
                return forwardedHeader.Split(',')[0].Trim();
            }

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        private string? GetApplicationTypeName(dynamic userData)
        {
            string? type = userData.OnlineApplicationResponse.ApplicationType?.ToString();
            return type switch
            {
                "1" => "HBA",
                "2" => userData.CarApplicationResponse?.Veh_Loan_Type == "Two Wheeler" ? "TW" : "CAR",
                "3" => "PCA",
                _ => null
            };
        }

        private async Task<string[]> GeneratePdfIfNeeded(int applicationId, string sourceFolderPath, string folderPath, bool isRejected, bool isApproved, string[] pdfFiles, string ip)
        {
            try
            {
                var session = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");
                if (session == null) throw new Exception("Session expired or invalid user context.");

                var (name, mobile, armyno, unitName, ApptName) = await _IonlineApplication1.GetCODetails(session.ProfileId);
                string generatedPdfPath = Path.Combine(sourceFolderPath, folderPath + "_Application.pdf");

                int result = await _pdfGenerator.CreatePdfForOnlineApplication(applicationId, generatedPdfPath, isRejected, isApproved, session.UserName, ip, name, mobile, armyno, unitName, ApptName);

                if (result == 1)
                {
                    pdfFiles = Directory.GetFiles(sourceFolderPath, "*.pdf")
                            .OrderBy(file =>
                            {
                                bool containsApplication = Path.GetFileName(file).Contains("Application");
                                return containsApplication ? 0 : 1;
                            })
                            .ThenBy(file => Path.GetFileName(file))
                            .ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF");
            }
            return pdfFiles;
        }

        private string PrepareMergedPdfPath(int applicationId, string armyNo)
        {
            string path = Path.Combine(_env.WebRootPath, "MergePdf");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return Path.Combine(path, $"App{applicationId}{armyNo}.pdf");
        }

        private JsonResult JsonError(string message) => Json(new { success = false, message });
        #endregion

        [HttpPost]
        public async Task<JsonResult> GetPdfFilePath(int applicationId)
        {
            if (!ModelState.IsValid)
            {
                return JsonError("Invalid Request.");
            }
            var userData = await _IonlineApplication1.GetApplicationDetails(applicationId);
            if (userData == null)
            {
                return Json(new { success = false, message = "Application not found." });
            }
            string applicationType = userData.OnlineApplicationResponse.ApplicationType.ToString();
            string applicationTypeName = "";
            if (string.IsNullOrEmpty(applicationType))
            {
                return Json(new { success = false, message = "Application type is not specified." });
            }
            else
            {
                if (applicationType == "1")
                {
                    applicationTypeName = "HBA";
                }
                else if (applicationType == "2")
                {
                    if (userData.CarApplicationResponse.Veh_Loan_Type == "Two Wheeler")
                    {
                        applicationTypeName = "TW";
                    }
                    else
                    {
                        applicationTypeName = "CAR";
                    }
                }
                else
                {
                    applicationTypeName = "PCA";
                }
            }
            string armyNo = userData.OnlineApplicationResponse.Number;
            if (string.IsNullOrEmpty(armyNo))
            {
                return Json(new { success = false, message = "Army number is not specified." });
            }
            string applicationIdStr = applicationId.ToString();
            if (string.IsNullOrEmpty(applicationIdStr))
            {
                return Json(new { success = false, message = "Application ID is not specified." });
            }
            string folderPath = applicationTypeName + armyNo + "_" + applicationIdStr;
            string mergepdfName = "App" + applicationIdStr + armyNo;
            string pdfFilePath = $"/MergePdf/{mergepdfName}.pdf";

            return Json(pdfFilePath);
        }

        [HttpPost]
        public async Task<JsonResult> GetApplicationDetails(int applicationId)
        {

            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }

            try
            {
                var applicationDetails = await _IonlineApplication1.GetApplicationDetails(applicationId);

                if (applicationDetails == null)
                {
                    return Json(new { success = false, message = "Application not found" });
                }

                var response = new
                {
                    success = true,
                    data = new
                    {
                        applicationId = applicationDetails.OnlineApplicationResponse.ApplicationId,
                        name = applicationDetails.OnlineApplicationResponse.ApplicantName,
                        armyNo = applicationDetails.OnlineApplicationResponse.Number,
                        unitName = applicationDetails.OnlineApplicationResponse.PresentUnit,
                        applicationType = applicationDetails.OnlineApplicationResponse.ApplicationTypeName,
                        accountNumber = applicationDetails.OnlineApplicationResponse.SalaryAcctNo,
                        ifscCode = applicationDetails.OnlineApplicationResponse.IfsCode,
                        appliedDate = applicationDetails.OnlineApplicationResponse.UpdatedOn,
                        rank = applicationDetails.OnlineApplicationResponse.DdlRank,
                        bank = applicationDetails.OnlineApplicationResponse.NameOfBank,
                    }
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while fetching application details" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetPdfFileByteByApplicationId(int applicationId)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid Request." });
            }
            try
            {
                var pdfFilePathResult = await GetPdfFilePath(applicationId);
                if (pdfFilePathResult.Value is not string pdfPath || string.IsNullOrEmpty(pdfPath))
                {
                    throw new FileNotFoundException("The specified file path is invalid or empty.");
                }

                string fullPath = Path.Combine(_env.WebRootPath, pdfPath.TrimStart('/'));

                if (!System.IO.File.Exists(fullPath))
                {
                    throw new FileNotFoundException("The specified file does not exist.", fullPath);
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);

                return Json(fileBytes);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred while fetching PDF" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveBase64ToFile(string base64String, string fileName)
        {
            string directoryPath = Path.Combine(_env.WebRootPath, "MergePdf");
            try
            {
                await _fileUtility.SaveBase64ToFileAsync(base64String, directoryPath, fileName);
                return Json(new { success = true, message = "File saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error saving file" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetDataByArmyNumber([FromBody] SessionUserDTO sessionUserDTO)
        {
            var applicationId = await _IonlineApplication1.GetLatestApplicationIdByArmyNo(sessionUserDTO.ArmyNo);
            if (applicationId == null)
            {
                return Json(new { success = false, message = "Application ID not found." });
            }
            try
            {
                DTOCommonOnlineApplicationResponse data = await _IonlineApplication1.GetApplicationDetailsByApplicationId(applicationId.Value);
                return Json(data.OnlineApplicationResponse);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
            
        }

        public async Task<JsonResult> GetDataByApplicationId(int applicationId)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid Request." });
            }
            DTOCommonOnlineApplicationResponse data = await _IonlineApplication1.GetApplicationDetailsByApplicationId(applicationId);
            return Json(data);
        }

        //[HttpPost]
        //public IActionResult HandleApplicationRedirect(int appId, string type)
        //{

        //    string redirectUrl = string.Empty;

        //    if (type.ToUpper() == "LOAN")
        //    {
        //        redirectUrl = Url.Action("OnlineApplication", "OnlineApplication");
        //        TempData["RedirectapplicationId"] = appId;
        //    }
        //    else if (type.ToUpper() == "MATURITY")
        //    {
        //        redirectUrl = Url.Action("OnlineApplication", "Claim");
        //        TempData["RedirectClaimapplicationId"] = appId;
        //    }

        //    return Json(new { success = true, redirectUrl = redirectUrl });
        //}


        [HttpPost]
        [ValidateAntiForgeryToken] // Ensure this matches your JS header

        public IActionResult HandleApplicationRedirect([FromForm] string EncryptedData)
        {
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

            string redirectUrl = string.Empty;

            if (searchParams.type.ToUpper() == "LOAN")
            {
                redirectUrl = Url.Action("OnlineApplication", "OnlineApplication");
                TempData["RedirectapplicationId"] = searchParams.ApplicationId;
            }
            else if (searchParams.type.ToUpper() == "MATURITY")
            {
                redirectUrl = Url.Action("OnlineApplication", "Claim");
                TempData["RedirectClaimapplicationId"] = searchParams.ApplicationId;
            }

            return Json(new { success = true, redirectUrl = redirectUrl });
        }

    }

}