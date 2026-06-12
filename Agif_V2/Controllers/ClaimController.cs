using Agif_V2.Helpers;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Agif_V2.Controllers
{
    public class ClaimController : Controller
    {
        public const string SessionClaimKeySalt = "_Salt";
        private readonly IClaimOnlineApplication _IClaimonlineApplication1;
        private readonly IClaimDocumentUpload _IclaimDocumentUpload;
        private readonly ClaimPdfGenerator _pdfGenerator;
        private readonly MergePdf _mergePdf;
        private readonly IWebHostEnvironment _env;
 
        private readonly PdfUpload _pdfUpload;
        private readonly IClaimAddress _ClaimAddress;
        private readonly IClaimAccount _ClaimAccount;
        private readonly FileUtility _fileUtility;
        private readonly Watermark _watermark;
        private readonly IModelStateLogger _modelStateLogger;


        public ClaimController(IClaimOnlineApplication OnlineApplication, IMasterOnlyTable MasterOnlyTable, ClaimPdfGenerator pdfGenerator, IWebHostEnvironment env, MergePdf mergePdf,IClaimDocumentUpload claimDocumentUpload, PdfUpload pdfUpload, IClaimAddress claimAddress, IClaimAccount claimAccount, FileUtility fileUtility, Watermark watermark, IModelStateLogger modelStateLogger)
        {

            _IClaimonlineApplication1 = OnlineApplication;      
            _pdfGenerator = pdfGenerator;
            _env = env;
            _mergePdf = mergePdf;
            this._IclaimDocumentUpload = claimDocumentUpload;
            this._pdfUpload = pdfUpload;
            _ClaimAddress = claimAddress;
            _ClaimAccount = claimAccount;
            _fileUtility = fileUtility;
            _watermark = watermark;
            _modelStateLogger = modelStateLogger;
        }

        public IActionResult MaturityLoanType()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CheckExistUser(string armyNumber, string Prefix, string Suffix, int appType)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }
            var existingUser = await _IClaimonlineApplication1.GetApplicationDetailsByArmyNo(armyNumber, Prefix, Suffix, appType);

            if (existingUser != null) 
            {
                return Json(new { exists = true }); 
            }
            else
            {
                return Json(new { exists = false });
            }
        }

        public async Task<JsonResult> DeleteExistingLoan(string armyNumber, string Prefix, string Suffix, int appType)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }
            bool result = await _IClaimonlineApplication1.DeleteExistingLoan(armyNumber, Prefix, Suffix, appType);

            if (result == true)
            {
                return Json(new { exists = true });
            }
            else
            {
                return Json(new { exists = false });
            }
        }


        public async Task<IActionResult> Upload()
        {
            // int applicationId = Convert.ToInt32(TempData["ClaimapplicationId"]);
            int applicationId = 32052;

            //bool application = await _IclaimDocumentUpload.CheckDocumentUploaded(applicationId);

            string FormType = await _IClaimonlineApplication1.GetFormType(applicationId);


            TempData.Keep("ClaimapplicationId");

            if (application)
            {
                TempData["Message"] = "You have already uploaded the Documents for this Application.";
                return RedirectToAction("ApplicationDetails", "Claim");
            }
            try
            {
                bool IsextensionOfService = await _IClaimonlineApplication1.CheckExtensionofservice(applicationId);

                TempData["ClaimIsextensionOfService"] = IsextensionOfService;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

            ClaimFileUploadViewModel ClaimfileUploadViewModel = new ClaimFileUploadViewModel();
            ClaimfileUploadViewModel.FormType= FormType;
            return View(ClaimfileUploadViewModel);
        }

        public async Task<IActionResult> ApplicationDetails()
        {
             int applicationId = Convert.ToInt32(TempData["ClaimapplicationId"]);
          //  int applicationId = 30044;

            TempData.Keep("ClaimapplicationId");

            if (applicationId == 0)
            {
                return NotFound();
            }

            var application = await _IClaimonlineApplication1.GetApplicationDetails(applicationId);
            if (application == null)
            {
                return NotFound();
            }

            ViewBag.Message = TempData["Message"];

            return View(application);
        }

        [HttpGet]
        public async Task<IActionResult> OnlineApplication()
        {

            string dd = AESEncrytDecry.GetSalt();
            HttpContext.Session.SetString(SessionClaimKeySalt, dd);
            ViewBag.hiddenClaimSalt = dd;
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }
            var Category = TempData["Category"] as string;
            var WithdrwalPurpose = TempData["WithdrwalPurpose"] as string;

            int id = TempData["RedirectClaimapplicationId"] is int applicationId ? applicationId : 0;

            TempData["CategoryNew"] = Category ?? string.Empty;

            TempData["WithdrwalPurposeNew"] = WithdrwalPurpose ?? string.Empty;


            var response = new DTOClaimCommonOnlineResponse();
            response = null;

            DTOClaimApplication DTOClaimApplication = new DTOClaimApplication();

            if (id != 0)
            {
                response = _IClaimonlineApplication1.GetApplicationAndApplicantType(id);
            }

            if (response != null)
            {
                DTOClaimApplication.Purpose = response.OnlineApplicationResponse.ApplicationType.ToString();
                DTOClaimApplication.Category = response.OnlineApplicationResponse.ApplicantType.ToString();                
            }

            TempData.Keep("Category");
            TempData.Keep("WithdrwalPurpose");

            return View(DTOClaimApplication);
        }

        public IActionResult Redirection(string Category, string PurposeOfWithdrwal)
        {
            TempData["Category"] = Category;
            TempData["WithdrwalPurpose"] = PurposeOfWithdrwal;
            return RedirectToAction("OnlineApplication");
        }


        [HttpPost]
        public async Task<IActionResult> OnlineApplication([FromForm] string EncryptedData)
        {

            string dd = HttpContext.Session.GetString(SessionClaimKeySalt);

            if (string.IsNullOrEmpty(dd))
            {
                dd = AESEncrytDecry.GetSalt();
                HttpContext.Session.SetString(SessionClaimKeySalt, dd);
            }

            ViewBag.hiddenClaimSalt = dd;

            if (string.IsNullOrEmpty(EncryptedData))
            {
                ModelState.AddModelError("", "Form data is missing or corrupted.");
                return View("OnlineApplication", new DTOClaimApplication());
            }

            DTOClaimApplication model;

            try
            {
                string secretKey = HttpContext.Session.GetString(SessionClaimKeySalt); // Example

                string decryptedJson = AESEncrytDecry.DecryptAES(EncryptedData, secretKey);


                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                options.Converters.Add(new Agif_V2.Helpers.UniversalFlexibleConverterFactory());

                model = System.Text.Json.JsonSerializer.Deserialize<DTOClaimApplication>(decryptedJson, options);

                var uploadedFiles = HttpContext.Request.Form.Files;

                if (uploadedFiles.Count > 0)
                {

                    if (model.EducationDetails != null)
     
                    {

                        model.EducationDetails.AttachPartIIOrder = uploadedFiles["EducationDetails.AttachPartIIOrder"];
                        model.EducationDetails.AttachBonafideLetter= uploadedFiles["EducationDetails.AttachBonafideLetter"];
                        model.EducationDetails.TotalExpenditureFile = uploadedFiles["EducationDetails.TotalExpenditureFile"];
                    }
                    if (model.PropertyRenovation != null)
                    {
                        model.PropertyRenovation.TotalExpenditureFile = uploadedFiles["PropertyRenovation.TotalExpenditureFile"];
                    }

                    if (model.Marriageward != null)
                    {
                        model.Marriageward.AttachPartIIOrder = uploadedFiles["Marriageward.AttachPartIIOrder"];
                        model.Marriageward.AttachInvitationcard = uploadedFiles["Marriageward.AttachInvitationcard"];
                    }

                }

                ModelState.Clear();

                TryValidateModel(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Security error: Failed to process the application payload.");
                return View("OnlineApplication", new DTOClaimApplication());
            }
            if (model.ClaimCommonData != null && model.ClaimCommonData.ResidualService > 1 && model.PropertyRenovation != null)
            {
                ModelState.AddModelError("ClaimCommonData.ResidualService", "Residual Service cannot exceed 2 years for Repair & Renovation.");
                return View(model);
            }
          

            await ValidateModelAsync(model);

            if (model.Category == "3") // OR
            {
                ModelState.Remove("ClaimCommonData.OldArmyPrefix");
                ModelState.Remove("ClaimCommonData.OldNumber");
                ModelState.Remove("ClaimCommonData.OldSuffix");
            }

            if (!ModelState.IsValid)
            {
                await _modelStateLogger.LogModelStateError(ModelState, HttpContext);
                return View("OnlineApplication", model);
            }
                

            var claimCommonModel = await SaveClaimCommonDataAsync(model);

            await SaveAddressAndAccountDetailsAsync(model, claimCommonModel.ApplicationId);

            string formType = await SubmitFormAsync(model, claimCommonModel.ApplicationId);

            TempData["ClaimapplicationId"] = claimCommonModel.ApplicationId;
            TempData["Message"] = "Your application has been saved successfully. Please upload the required document to proceed.";
            return RedirectToAction("Upload", "Claim");
        }

        private async Task<bool> ValidateModelAsync(DTOClaimApplication model)
        {            
            bool isValid = true;

            isValid &= ValidateSection(model.ClaimCommonData, "ClaimCommonData");
            isValid &= ValidateSection(model.AddressDetails, "AddressDetails");
            isValid &= ValidateSection(model.AccountDetails, "AccountDetails");
            isValid &= ValidateSection(model.EducationDetails, "EducationDetails");
            isValid &= ValidateSection(model.Marriageward, "Marriageward");
            isValid &= ValidateSection(model.PropertyRenovation, "PropertyRenovation");
            isValid &= ValidateSection(model.SplWaiver, "SplWaiver");


            if (model.EducationDetails != null)
                isValid &= await ValidateFormFilesAsync(model.EducationDetails, "EducationDetails", new Dictionary<string, string>
            {
                {"AttachPartIIOrder", "AttachPartIIOrder"},
                {"AttachBonafideLetter", "AttachBonafideLetter"},
                {"TotalExpenditureFile", "TotalExpenditureFile"}
            });
                else if (model.Marriageward != null)
                    isValid &= await ValidateFormFilesAsync(model.Marriageward, "Marriageward", new Dictionary<string, string>
            {
                {"AttachPartIIOrder", "AttachPartIIOrder"},
                {"AttachInvitationcard", "AttachInvitationcard"}
            });
                else if (model.PropertyRenovation != null)
                    isValid &= await ValidateFormFilesAsync(model.PropertyRenovation, "PropertyRenovation", new Dictionary<string, string>
            {
                {"TotalExpenditureFile", "TotalExpenditureFile"}
            });
                else if (model.SplWaiver != null)
                    isValid &= await ValidateFormFilesAsync(model.SplWaiver, "SplWaiver", new Dictionary<string, string>
            {
                {"TotalExpenditureFile", "TotalExpenditureFile"},
                {"OtherReasonPdf", "OtherReasonPdf"}
            });

            return isValid;
        }

        private bool ValidateSection(object section, string sectionName)
        {
            if (section == null) return true;

            var context = new ValidationContext(section);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(section, context, results, true))
            {
                foreach (var result in results)
                {
                    string propertyName = result.MemberNames?.FirstOrDefault();
                    string errorKey = string.IsNullOrEmpty(propertyName)
                        ? sectionName
                        : $"{sectionName}.{propertyName}";
                    ModelState.AddModelError(errorKey, result.ErrorMessage);
                }
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateFormFilesAsync(object form, string formPrefix, Dictionary<string, string> files)
        {
            bool isValid = true;

            foreach (var fileProp in files)
            {
                var file = form.GetType().GetProperty(fileProp.Key)?.GetValue(form) as IFormFile;
                if (file == null) continue;

                string errorMessage;
                if (!_IClaimonlineApplication1.ValidateFileUpload(file, out errorMessage))
                {
                    ModelState.AddModelError($"{formPrefix}.{fileProp.Value}", errorMessage);
                    isValid = false;
                }
                if (await _pdfUpload.CheckIfPdfPasswordProtected(file))
                {
                    errorMessage = "Only valid, Non-password-protected PDF files are allowed.";
                    ModelState.AddModelError($"{formPrefix}.{fileProp.Value}", errorMessage);
                    isValid = false;
                }


            }

            return isValid;
        }

        private async Task<ClaimCommonModel> SaveClaimCommonDataAsync(DTOClaimApplication model)
        {
            string? ip = GetClientIp();
            if (model.ClaimCommonData == null) return new ClaimCommonModel();

            model.ClaimCommonData.ApplicantType = int.Parse(model.Category);
            model.ClaimCommonData.WithdrawPurpose = int.Parse(model.Purpose);
            model.ClaimCommonData.IOArmyNo = string.IsNullOrEmpty(model.COArmyNo) ? "" : model.COArmyNo;
            model.ClaimCommonData.IPAddress = ip;

            if (model.ClaimCommonData.ApplicantType == 3)
            {
                model.ClaimCommonData.OldArmyPrefix = 0;
                model.ClaimCommonData.OldNumber = string.Empty;
                model.ClaimCommonData.OldSuffix = string.Empty;
            }

            if (string.IsNullOrEmpty(model.ClaimCommonData.pcda_AcctNo))
            {
                model.ClaimCommonData.pcda_AcctNo = string.Empty;
            }
            if (string.IsNullOrEmpty(model.ClaimCommonData.pcda_pao))
            {
                model.ClaimCommonData.pcda_pao = string.Empty;
            }
            return await _IClaimonlineApplication1.AddWithReturn(model.ClaimCommonData);
        }

        private async Task SaveAddressAndAccountDetailsAsync(DTOClaimApplication model, int applicationId)
        {
            if (model.AddressDetails != null)
            {
                model.AddressDetails.ApplicationId = applicationId;
                await _ClaimAddress.Add(model.AddressDetails);
            }

            if (model.AccountDetails != null)
            {
                model.AccountDetails.ApplicationId = applicationId;
                await _ClaimAccount.Add(model.AccountDetails);
            }
        }

        private async Task<string> SubmitFormAsync(DTOClaimApplication model, int applicationId)
        {
            if (model.EducationDetails != null)
            {
                await _IClaimonlineApplication1.submitApplication(model, "ED", applicationId);
                return "ED";
            }

            if (model.Marriageward != null)
            {
                await _IClaimonlineApplication1.submitApplication(model, "MW", applicationId);
                return "MW";
            }

            if (model.PropertyRenovation != null)
            {
                await _IClaimonlineApplication1.submitApplication(model, "PR", applicationId);
                return "PR";
            }

            await _IClaimonlineApplication1.submitApplication(model, "SP", applicationId);
            return "SP";
        }




        [HttpPost]
        public async Task<IActionResult> Upload(ClaimFileUploadViewModel model, string formType, int applicationId)
        {
            try
            {
                TempData.Keep("ClaimapplicationId");

            var files = GetUploadedFiles(model);

            if (!files.Any())
            {
                ModelState.AddModelError("", "Please upload at least one file.");
                return View("Upload", model);
            }

            foreach (var file in files)
            {
                await ValidateFile(file);
            }

            if (!ModelState.IsValid)
            {
                await _modelStateLogger.LogModelStateError(ModelState, HttpContext);
                return View("Upload", model);
            }

            bool success = await _IClaimonlineApplication1.ProcessFileUploads(files, formType, applicationId);

            if (!success)
            {
                ModelState.AddModelError("", "File upload failed. Please try again.");
                return View("Upload", model);
            }

            return RedirectToAction("ApplicationDetails", "Claim");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

        private List<IFormFile> GetUploadedFiles(ClaimFileUploadViewModel model)
        {
            var files = new List<IFormFile>();
            if (model.CancelledCheque != null) files.Add(model.CancelledCheque);
            if (model.PaySlipPdf != null) files.Add(model.PaySlipPdf);
            if (model.SpdocusPdf != null) files.Add(model.SpdocusPdf);
            if (model.SeviceExtnPdf != null) files.Add(model.SeviceExtnPdf);
            return files;
        }

        private async Task ValidateFile(IFormFile file)
        {
            if (file.ContentType != "application/pdf")
            {
                ModelState.AddModelError(file.Name, "Only PDF files are allowed.");
            }

            if (file.Length > 150 * 1024)
            {
                ModelState.AddModelError(file.Name, "File size must not exceed 150 KB.");
            }

            if (file.Length > 1 * 1024 * 1024)
            {
                ModelState.AddModelError(file.Name, "File size must not exceed 1 MB.");
            }

            if (await _pdfUpload.CheckIfPdfPasswordProtected(file))
            {
                ModelState.AddModelError(file.Name, "Only valid, Non-password-protected PDF files are allowed.");
            }
            //if (await _pdfUpload.IsValidPdfFile(file))
            //{
            //    ModelState.AddModelError(file.Name, "Not A Valid Pdf");
            //}
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
        public async Task<JsonResult> MergePdf(int applicationId, bool isRejected, bool isApproved)
        {

            try
            {
                string ip = GetClientIp();
                var userData = await _IClaimonlineApplication1.GetApplicationDetails(applicationId);
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
                        applicationTypeName = "ED";
                    }
                    else if (applicationType == "2")
                    {
                        applicationTypeName = "MW";
                    }
                    else if (applicationType == "3")
                    {
                        applicationTypeName = "PR";
                    }
                    else if (applicationType == "4")
                        applicationTypeName = "SP";
                }

                string armyNo = userData.OnlineApplicationResponse.Number;
                if (string.IsNullOrEmpty(armyNo))
                {
                    return Json(new { success = false, message = "Army number is not specified." });
                }

                string applicationIdStr = applicationId.ToString();
                string folderPath = applicationTypeName + "_" + armyNo + "_" + applicationIdStr;
                string sourceFolderPath = Path.Combine(_env.WebRootPath, "ClaimTempUploads", folderPath);


                if (!Directory.Exists(sourceFolderPath))
                {
                    return Json(new { success = false, message = $"Source folder not found: {sourceFolderPath}" });
                }

                string[] pdfFiles = Directory.GetFiles(sourceFolderPath, "*.pdf");

                if (pdfFiles.Length == 0)
                {
                    return Json(new { success = false, message = "No PDF files found in the specified folder." });
                }

                string pdfName = folderPath + "_Application";
                var generatedPdfPath = Path.Combine(sourceFolderPath, pdfName + ".pdf");

                try
                {
                    SessionUserDTO? dTOTempSession = Helpers.SessionExtensions.GetObject<SessionUserDTO>(HttpContext.Session, "User");

                    if (dTOTempSession == null)
                    {
                        return Json(new { success = false, message = "Session expired or invalid user context." });
                    }

                    var (name, mobile, armyno) = await _IClaimonlineApplication1.GetCODetails(dTOTempSession.ProfileId);

                    var data = await _pdfGenerator.CreatePdfForOnlineApplication(applicationId, generatedPdfPath, isRejected, isApproved, dTOTempSession.UserName, ip, name, mobile, armyno);


                    if (data == 1)
                    {
                    

                        pdfFiles = Directory.GetFiles(sourceFolderPath, "*.pdf")
                           .OrderBy(file =>
                           {
                               bool containsApplication = Path.GetFileName(file).Contains("Application");
                               return containsApplication ? 0 : 1;
                           })
                           .ThenBy(file => Path.GetFileName(file))  // After prioritizing, order by the filename
                           .ToArray();
                    }
                }
                catch (Exception pdfGenEx)
                {
                    Console.WriteLine($"Error generating PDF: {pdfGenEx.Message}");
                }

                string tempUploadsPath = Path.Combine(_env.WebRootPath, "ClaimMergePdf");
                if (!Directory.Exists(tempUploadsPath))
                {
                    Directory.CreateDirectory(tempUploadsPath);
                }

                string MergePdfName = "App" + applicationIdStr + armyNo;
                string mergedPdfPath = Path.Combine(tempUploadsPath, MergePdfName + ".pdf");
                ViewBag.MergedPdfPath = mergedPdfPath;
                bool mergeResult = await _mergePdf.MergePdfFiles(pdfFiles, mergedPdfPath);


                if (mergeResult)
                {
                    string relativePath = mergedPdfPath.Replace(_env.WebRootPath, "").Replace("\\", "/");

                    await _IClaimonlineApplication1.UpdateMergePdfStatus(applicationId, true);
                    return Json(new
                    {
                        success = true,
                        message = "PDFs merged successfully.",
                        mergedFilePath = relativePath,
                        fullPath = mergedPdfPath,
                        totalFiles = pdfFiles.Length
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to merge PDF files." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error occurred while merging PDFs" });
            }
        }


        public async Task<JsonResult> GetPdfFilePath(int applicationId)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }
            var userData = await _IClaimonlineApplication1.GetApplicationDetails(applicationId);
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
                    applicationTypeName = "ED";
                }
                else if (applicationType == "2")
                {
                    applicationTypeName = "MW";
                }
                else if (applicationType == "3")
                {
                    applicationTypeName = "PR";
                }
                else if (applicationType == "4")
                    applicationTypeName = "SP";
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
            string folderPath = applicationTypeName + "_" + armyNo + "_" + applicationIdStr;
            string mergepdfName = "App" + applicationIdStr + armyNo;
            string pdfFilePath = $"/ClaimMergePdf/{mergepdfName}.pdf";
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
                var applicationDetails = await _IClaimonlineApplication1.GetApplicationDetails(applicationId);

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
                        rank=applicationDetails.OnlineApplicationResponse.DdlRank,
                        unitName = applicationDetails.OnlineApplicationResponse.PresentUnit,
                        applicationType = applicationDetails.OnlineApplicationResponse.ApplicationTypeName,
                        accountNumber = applicationDetails.OnlineApplicationResponse.SalaryAcctNo,
                        ifscCode = applicationDetails.OnlineApplicationResponse.IfsCode,
                        appliedDate = applicationDetails.OnlineApplicationResponse.UpdatedOn,
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
        public JsonResult InfoBeforeUpload(string applicationId)
        {
            if (string.IsNullOrWhiteSpace(applicationId) || applicationId == "0")
            {
                return Json(new { success = false, message = "Application ID is required." });
            }

            var coDetails = _IClaimonlineApplication1.GetUnitByApplicationId(int.Parse(applicationId));
            var data = coDetails.Result?.OnlineApplicationResponse;


            if (data == null)
            {
                return Json(new { success = false, message = "No data found for the provided Application ID." });
            }

            string CoArmyNumber = data.Number ?? string.Empty;
            string CoRank = data.DdlRank ?? string.Empty;
            string CoUnit = data.PresentUnit ?? string.Empty;
            string CoName = data.CoName ?? string.Empty;

            var message = $"Application will be forwarded to your Unit Commander {CoArmyNumber} {CoRank} {CoName}, {CoUnit}";
            return Json(new { success = true, message = message });
        }


        [HttpPost]
        public async Task<JsonResult> SaveBase64ToFile(string base64String, string fileName)
        {
            string directoryPath = Path.Combine(_env.WebRootPath, "ClaimMergePdf");
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
        public async Task<JsonResult> GetDataByArmyNumber(string ArmyNo)
        {
            var applicationId = await _IClaimonlineApplication1.GetLatestApplicationIdByArmyNo(ArmyNo);
            if (applicationId == null)
            {
                return Json(new { success = false, message = "Application ID not found." });
            }
            try
            {
                DTOClaimCommonOnlineResponse data = await _IClaimonlineApplication1.GetApplicationDetailsByApplicationId(applicationId.Value);
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
            DTOClaimCommonOnlineResponse data = await _IClaimonlineApplication1.GetApplicationDetailsByApplicationId(applicationId);
            return Json(data);
        }
    }
}
