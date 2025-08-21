
using Agif_V2.Helpers;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Agif_V2.Controllers
{
    public class ClaimController : Controller
    {
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


        public ClaimController(IClaimOnlineApplication OnlineApplication, IMasterOnlyTable MasterOnlyTable, ClaimPdfGenerator pdfGenerator, IWebHostEnvironment env, MergePdf mergePdf,IClaimDocumentUpload claimDocumentUpload, PdfUpload pdfUpload, IClaimAddress claimAddress, IClaimAccount claimAccount, FileUtility fileUtility, Watermark watermark)
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
        }

        public IActionResult MaturityLoanType()
        {
            return View();
        }

        public async Task<JsonResult> CheckExistUser(string armyNumber, string Prefix, string Suffix, int appType)
        {
            if (!ModelState.IsValid)
            {
                return Json("Invalid Request.");
            }
            var existingUser = await _IClaimonlineApplication1.GetApplicationDetailsByArmyNo(armyNumber, Prefix, Suffix, appType);

            if (existingUser != null) // Check if the user exists
            {
                return Json(new { exists = true }); // User exists
            }
            else
            {
                return Json(new { exists = false }); // User does not exist
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
            int applicationId = Convert.ToInt32(TempData["ClaimapplicationId"]);

            bool application = await _IclaimDocumentUpload.CheckDocumentUploaded(applicationId);

            string FormType = await _IClaimonlineApplication1.GetFormType(applicationId);


            TempData.Keep("ClaimapplicationId");

            if (application)
            {
                TempData["Message"] = "You have already uploaded the Documents for this Application.";
                return RedirectToAction("ApplicationDetails", "Claim");
            }

            bool IsextensionOfService = await _IClaimonlineApplication1.CheckExtensionofservice(applicationId);

            TempData["ClaimIsextensionOfService"] = IsextensionOfService;

            ClaimFileUploadViewModel ClaimfileUploadViewModel = new ClaimFileUploadViewModel();
            ClaimfileUploadViewModel.FormType= FormType;
            return View(ClaimfileUploadViewModel);
        }

        public async Task<IActionResult> ApplicationDetails()
        {
            int applicationId = Convert.ToInt32(TempData["ClaimapplicationId"]);

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
            var Category = TempData["Category"] as string;
            var WithdrwalPurpose = TempData["WithdrwalPurpose"] as string;


            TempData["CategoryNew"] = EncryptDecrypt.DecryptionData(Category);

            TempData["WithdrwalPurposeNew"] = EncryptDecrypt.DecryptionData(WithdrwalPurpose);


            TempData.Keep("Category");
            TempData.Keep("WithdrwalPurpose");

            DTOClaimApplication DTOClaimApplication = new DTOClaimApplication();
            return View(DTOClaimApplication);
        }

        public IActionResult Redirection(string Category, string PurposeOfWithdrwal)
        {
            string AppCategory = EncryptDecrypt.EncryptionData(Category);
            string WithdrwalPurpose = EncryptDecrypt.EncryptionData(PurposeOfWithdrwal);

            TempData["Category"] = AppCategory;
            TempData["WithdrwalPurpose"] = WithdrwalPurpose;
            return RedirectToAction("OnlineApplication");
        }


        //public async Task<IActionResult> SubmitApplication(DTOClaimApplication model)
        //{
        //    string formType = string.Empty;
        //    if (model.EducationDetails != null)
        //    {
        //        var educationContext = new ValidationContext(model.EducationDetails);
        //        var educationValidationResults = new List<ValidationResult>();

        //        if (!Validator.TryValidateObject(model.EducationDetails, educationContext, educationValidationResults, true))
        //        {
        //            foreach (var result in educationValidationResults)
        //            {
        //                string propertyName = result.MemberNames?.FirstOrDefault();
        //                string errorKey = string.IsNullOrEmpty(propertyName)
        //                    ? "EducationDetails"
        //                    : $"EducationDetails.{propertyName}";
        //                ModelState.AddModelError(errorKey, result.ErrorMessage);
        //            }
        //        }
        //        // File Upload Validation for EducationDetails

        //        if (model.EducationDetails.AttachPartIIOrder != null)
        //        {
        //            string errorMessage;
        //            if (!_IClaimonlineApplication1.ValidateFileUpload(model.EducationDetails.AttachPartIIOrder, out errorMessage))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachPartIIOrder", errorMessage);
        //            }
        //            if (!await _pdfUpload.IsValidPdfFile(model.EducationDetails.AttachPartIIOrder))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachPartIIOrder", "File is not a valid PDF or appears to be a disguised file type.");
        //            }
        //            if (await _pdfUpload.IsPdfPasswordProtected(model.EducationDetails.AttachPartIIOrder))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachPartIIOrder", "Password-protected PDFs are not allowed.");
        //            }
        //            if (await _pdfUpload.ContainsMaliciousPdfContent(model.EducationDetails.AttachPartIIOrder))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachPartIIOrder", "PDF contains potentially malicious content.");
        //            }

        //        }

        //        if (model.EducationDetails.AttachBonafideLetter != null)
        //        {
        //            string errorMessage;
        //            if (!_IClaimonlineApplication1.ValidateFileUpload(model.EducationDetails.AttachBonafideLetter, out errorMessage))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachInvitationcard", errorMessage);
        //            }

        //            if (!await _pdfUpload.IsValidPdfFile(model.EducationDetails.AttachBonafideLetter))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachInvitationcard", "File is not a valid PDF or appears to be a disguised file type.");
        //            }

        //            if (await _pdfUpload.IsPdfPasswordProtected(model.EducationDetails.AttachBonafideLetter))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachInvitationcard", "Password-protected PDFs are not allowed.");
        //            }

        //            if (await _pdfUpload.ContainsMaliciousPdfContent(model.EducationDetails.AttachBonafideLetter))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachInvitationcard", "PDF contains potentially malicious content.");
        //            }
        //        }

        //        if (model.EducationDetails.TotalExpenditureFile != null)
        //        {
        //            string errorMessage;
        //            if (!_IClaimonlineApplication1.ValidateFileUpload(model.EducationDetails.TotalExpenditureFile, out errorMessage))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachPartIIOrder", errorMessage);
        //            }
        //            if (!await _pdfUpload.IsValidPdfFile(model.EducationDetails.TotalExpenditureFile))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachPartIIOrder", "File is not a valid PDF or appears to be a disguised file type.");
        //            }

        //            if (await _pdfUpload.IsPdfPasswordProtected(model.EducationDetails.TotalExpenditureFile))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachPartIIOrder", "Password-protected PDFs are not allowed.");
        //            }

        //            if (await _pdfUpload.ContainsMaliciousPdfContent(model.EducationDetails.TotalExpenditureFile))
        //            {
        //                ModelState.AddModelError("EducationDetails.AttachPartIIOrder", "PDF contains potentially malicious content.");
        //            }
        //        }


        //    }
        //    else if (model.Marriageward != null)
        //    {
        //        var pcaValidationContext = new ValidationContext(model.Marriageward);
        //        var pcaValidationResults = new List<ValidationResult>();
        //        if (!Validator.TryValidateObject(model.Marriageward, pcaValidationContext, pcaValidationResults, true))
        //        {
        //            foreach (var result in pcaValidationResults)
        //            {
        //                string propertyName = result.MemberNames?.FirstOrDefault();
        //                string errorKey = string.IsNullOrEmpty(propertyName)
        //                    ? "Marriageward"
        //                    : $"Marriageward.{propertyName}";
        //                ModelState.AddModelError(errorKey, result.ErrorMessage);
        //            }

        //        }
        //        if (model.Marriageward.AttachPartIIOrder != null)
        //        {
        //            string errorMessage;
        //            if (!_IClaimonlineApplication1.ValidateFileUpload(model.Marriageward.AttachPartIIOrder, out errorMessage))
        //            {
        //                ModelState.AddModelError("Marriageward.AttachPartIIOrder", errorMessage);
        //            }

        //            if (!await _pdfUpload.IsValidPdfFile(model.Marriageward.AttachPartIIOrder))
        //            {
        //                ModelState.AddModelError("Marriageward.AttachPartIIOrder", "File is not a valid PDF or appears to be a disguised file type.");
        //            }

        //            if (await _pdfUpload.IsPdfPasswordProtected(model.Marriageward.AttachPartIIOrder))
        //            {
        //                ModelState.AddModelError("Marriageward.AttachPartIIOrder", "Password-protected PDFs are not allowed.");
        //            }

        //            if (await _pdfUpload.ContainsMaliciousPdfContent(model.Marriageward.AttachPartIIOrder))
        //            {
        //                ModelState.AddModelError("Marriageward.AttachPartIIOrder", "PDF contains potentially malicious content.");
        //            }
        //        }

        //        if (model.Marriageward.AttachInvitationcard != null)
        //        {
        //            string errorMessage;
        //            if (!_IClaimonlineApplication1.ValidateFileUpload(model.Marriageward.AttachInvitationcard, out errorMessage))
        //            {
        //                ModelState.AddModelError("Marriageward.AttachInvitationcard", errorMessage);
        //            }
        //            if (!await _pdfUpload.IsValidPdfFile(model.Marriageward.AttachInvitationcard))
        //            {
        //                ModelState.AddModelError("Marriageward.AttachInvitationcard", "File is not a valid PDF or appears to be a disguised file type.");
        //            }

        //            if (await _pdfUpload.IsPdfPasswordProtected(model.Marriageward.AttachInvitationcard))
        //            {
        //                ModelState.AddModelError("Marriageward.AttachInvitationcard", "Password-protected PDFs are not allowed.");
        //            }

        //            if (await _pdfUpload.ContainsMaliciousPdfContent(model.Marriageward.AttachInvitationcard))
        //            {
        //                ModelState.AddModelError("Marriageward.AttachInvitationcard", "PDF contains potentially malicious content.");
        //            }
        //        }
        //    }
        //    else if (model.PropertyRenovation != null)
        //    {
        //        var hbaValidationContext = new ValidationContext(model.PropertyRenovation);
        //        var hbaValidationResults = new List<ValidationResult>();
        //        if (!Validator.TryValidateObject(model.PropertyRenovation, hbaValidationContext, hbaValidationResults, true))
        //        {
        //            foreach (var result in hbaValidationResults)
        //            {
        //                string propertyName = result.MemberNames?.FirstOrDefault();
        //                string errorKey = string.IsNullOrEmpty(propertyName)
        //                    ? "PropertyRenovation"
        //                    : $"PropertyRenovation.{propertyName}";
        //                ModelState.AddModelError(errorKey, result.ErrorMessage);
        //            }
        //        }

        //        if (model.PropertyRenovation.TotalExpenditureFile != null)
        //        {
        //            string errorMessage;
        //            if (!_IClaimonlineApplication1.ValidateFileUpload(model.PropertyRenovation.TotalExpenditureFile, out errorMessage))
        //            {
        //                ModelState.AddModelError("PropertyRenovation.TotalExpenditureFile", errorMessage);
        //            }

        //            if (!await _pdfUpload.IsValidPdfFile(model.PropertyRenovation.TotalExpenditureFile))
        //            {
        //                ModelState.AddModelError("PropertyRenovation.TotalExpenditureFile", "File is not a valid PDF or appears to be a disguised file type.");
        //            }

        //            if (await _pdfUpload.IsPdfPasswordProtected(model.PropertyRenovation.TotalExpenditureFile))
        //            {
        //                ModelState.AddModelError("PropertyRenovation.TotalExpenditureFile", "Password-protected PDFs are not allowed.");
        //            }

        //            if (await _pdfUpload.ContainsMaliciousPdfContent(model.PropertyRenovation.TotalExpenditureFile))
        //            {
        //                ModelState.AddModelError("PropertyRenovation.TotalExpenditureFile", "PDF contains potentially malicious content.");
        //            }
        //        }
        //    }
        //    else if (model.SplWaiver != null)
        //    {
        //        var splWaiverValidationContext = new ValidationContext(model.SplWaiver);
        //        var splWaiverValidationResults = new List<ValidationResult>();
        //        if (!Validator.TryValidateObject(model.SplWaiver, splWaiverValidationContext, splWaiverValidationResults, true))
        //        {
        //            foreach (var result in splWaiverValidationResults)
        //            {
        //                string propertyName = result.MemberNames?.FirstOrDefault();
        //                string errorKey = string.IsNullOrEmpty(propertyName)
        //                    ? "SplWaiver"
        //                    : $"SplWaiver.{propertyName}";
        //                ModelState.AddModelError(errorKey, result.ErrorMessage);
        //            }
        //        }
        //        if (model.SplWaiver.TotalExpenditureFile != null)
        //        {
        //            string errorMessage;
        //            if (!_IClaimonlineApplication1.ValidateFileUpload(model.SplWaiver.TotalExpenditureFile, out errorMessage))
        //            {
        //                ModelState.AddModelError("SplWaiver.TotalExpenditureFile", errorMessage);
        //            }

        //            if (!await _pdfUpload.IsValidPdfFile(model.SplWaiver.TotalExpenditureFile))
        //            {
        //                ModelState.AddModelError("SplWaiver.TotalExpenditureFile", "File is not a valid PDF or appears to be a disguised file type.");
        //            }

        //            if (await _pdfUpload.IsPdfPasswordProtected(model.SplWaiver.TotalExpenditureFile))
        //            {
        //                ModelState.AddModelError("SplWaiver.TotalExpenditureFile", "Password-protected PDFs are not allowed.");
        //            }

        //            if (await _pdfUpload.ContainsMaliciousPdfContent(model.SplWaiver.TotalExpenditureFile))
        //            {
        //                ModelState.AddModelError("SplWaiver.TotalExpenditureFile", "PDF contains potentially malicious content.");
        //            }
        //        }

        //        if (model.SplWaiver.OtherReasonPdf != null)
        //        {
        //            string errorMessage;
        //            if (!_IClaimonlineApplication1.ValidateFileUpload(model.SplWaiver.OtherReasonPdf, out errorMessage))
        //            {
        //                ModelState.AddModelError("SplWaiver.OtherReasonPdf", errorMessage);
        //            }

        //            if (!await _pdfUpload.IsValidPdfFile(model.SplWaiver.OtherReasonPdf))
        //            {
        //                ModelState.AddModelError("SplWaiver.OtherReasonPdf", "File is not a valid PDF or appears to be a disguised file type.");
        //            }

        //            if (await _pdfUpload.IsPdfPasswordProtected(model.SplWaiver.OtherReasonPdf))
        //            {
        //                ModelState.AddModelError("SplWaiver.OtherReasonPdf", "Password-protected PDFs are not allowed.");
        //            }

        //            if (await _pdfUpload.ContainsMaliciousPdfContent(model.SplWaiver.OtherReasonPdf))
        //            {
        //                ModelState.AddModelError("SplWaiver.OtherReasonPdf", "PDF contains potentially malicious content.");
        //            }
        //        }

        //    }


        //    if (model.ClaimCommonData != null)
        //    {
        //        var commonDataValidationContext = new ValidationContext(model.ClaimCommonData);
        //        var commonDataValidationResults = new List<ValidationResult>();
        //        if (!Validator.TryValidateObject(model.ClaimCommonData, commonDataValidationContext, commonDataValidationResults, true))
        //        {
        //            foreach (var result in commonDataValidationResults)
        //            {
        //                string propertyName = result.MemberNames?.FirstOrDefault();
        //                string errorKey = string.IsNullOrEmpty(propertyName)
        //                    ? "ClaimCommonData"
        //                    : $"ClaimCommonData.{propertyName}";
        //                ModelState.AddModelError(errorKey, result.ErrorMessage);
        //            }
        //        }
        //    }

        //    if (model.AddressDetails != null)
        //    {
        //        var addressValidationContext = new ValidationContext(model.AddressDetails);
        //        var addressValidationResults = new List<ValidationResult>();
        //        if (!Validator.TryValidateObject(model.AddressDetails, addressValidationContext, addressValidationResults, true))
        //        {
        //            foreach (var result in addressValidationResults)
        //            {
        //                string propertyName = result.MemberNames?.FirstOrDefault();
        //                string errorKey = string.IsNullOrEmpty(propertyName)
        //                    ? "AddressDetails"
        //                    : $"AddressDetails.{propertyName}";
        //                ModelState.AddModelError(errorKey, result.ErrorMessage);
        //            }
        //        }
        //    }

        //    if (model.AccountDetails != null)
        //    {
        //        var accountValidationContext = new ValidationContext(model.AccountDetails);
        //        var accountValidationResults = new List<ValidationResult>();
        //        if (!Validator.TryValidateObject(model.AccountDetails, accountValidationContext, accountValidationResults, true))
        //        {
        //            foreach (var result in accountValidationResults)
        //            {
        //                string propertyName = result.MemberNames?.FirstOrDefault();
        //                string errorKey = string.IsNullOrEmpty(propertyName)
        //                    ? "AccountDetails"
        //                    : $"AccountDetails.{propertyName}";
        //                ModelState.AddModelError(errorKey, result.ErrorMessage);
        //            }
        //        }
        //    }

        //    if (!ModelState.IsValid)
        //        return View("OnlineApplication", model);
        //    else
        //    {
        //        ClaimCommonModel claimCommonModel = new ClaimCommonModel();
        //        ClaimAddressDetailsModel addressDetails = new ClaimAddressDetailsModel();
        //        ClaimAccountDetailsModel accountDetails = new ClaimAccountDetailsModel();

        //        if (model.ClaimCommonData != null)
        //        {
        //            model.ClaimCommonData.ApplicantType = int.Parse(model.Category);
        //            model.ClaimCommonData.WithdrawPurpose = int.Parse(model.Purpose);
        //            model.ClaimCommonData.IOArmyNo = string.IsNullOrEmpty(model.COArmyNo) ? "" : model.COArmyNo;
        //            claimCommonModel = await _IClaimonlineApplication1.AddWithReturn(model.ClaimCommonData);
        //        }

        //        if (model.AddressDetails != null)
        //        {
        //            model.AddressDetails.ApplicationId = claimCommonModel.ApplicationId;
        //            await _ClaimAddress.Add(model.AddressDetails);
        //        }

        //        if (model.AccountDetails != null)
        //        {
        //            model.AccountDetails.ApplicationId = claimCommonModel.ApplicationId;
        //            await _ClaimAccount.Add(model.AccountDetails);
        //        }


        //        if (model.EducationDetails != null)
        //        {
        //            formType = "ED";
        //            bool result = await _IClaimonlineApplication1.submitApplication(model, "ED", claimCommonModel.ApplicationId);


        //        }
        //        else if (model.Marriageward != null)
        //        {
        //            formType = "MW";
        //            bool result = await _IClaimonlineApplication1.submitApplication(model, "MW", claimCommonModel.ApplicationId);
        //        }
        //        else if (model.PropertyRenovation != null)
        //        {
        //            formType = "PR";
        //            bool result = await _IClaimonlineApplication1.submitApplication(model, "PR", claimCommonModel.ApplicationId);
        //        }
        //        else
        //        {
        //            formType = "SP";
        //            bool result = await _IClaimonlineApplication1.submitApplication(model, "SP", claimCommonModel.ApplicationId);

        //        }

        //        TempData["ClaimapplicationId"] = claimCommonModel.ApplicationId;
        //        TempData["Message"] = "Your application has been saved successfully. Please upload the required document to proceed.";
        //        return RedirectToAction("Upload", "Claim");

        //    }



        //}
        [HttpPost]
        public async Task<IActionResult> OnlineApplication(DTOClaimApplication model)
        {
            if (!await ValidateModelAsync(model))
                return View("OnlineApplication", model);

            // Save common data
            var claimCommonModel = await SaveClaimCommonDataAsync(model);

            // Save address & account details
            await SaveAddressAndAccountDetailsAsync(model, claimCommonModel.ApplicationId);

            // Submit the specific form type
            string formType = await SubmitFormAsync(model, claimCommonModel.ApplicationId);

            TempData["ClaimapplicationId"] = claimCommonModel.ApplicationId;
            TempData["Message"] = "Your application has been saved successfully. Please upload the required document to proceed.";
            return RedirectToAction("Upload", "Claim");
        }

        private async Task<bool> ValidateModelAsync(DTOClaimApplication model)
        {
            bool isValid = true;

            // Validate each section
            isValid &= ValidateSection(model.ClaimCommonData, "ClaimCommonData");
            isValid &= ValidateSection(model.AddressDetails, "AddressDetails");
            isValid &= ValidateSection(model.AccountDetails, "AccountDetails");
            isValid &= ValidateSection(model.EducationDetails, "EducationDetails");
            isValid &= ValidateSection(model.Marriageward, "Marriageward");
            isValid &= ValidateSection(model.PropertyRenovation, "PropertyRenovation");
            isValid &= ValidateSection(model.SplWaiver, "SplWaiver");


            // Form-specific validation
            if (model.EducationDetails != null)
                isValid &= await ValidateFormFilesAsync(model.EducationDetails, "EducationDetails", new Dictionary<string, string>
            {
                {"AttachPartIIOrder", "AttachPartIIOrder"},
                {"AttachBonafideLetter", "AttachBonafideLetter"},
                {"TotalExpenditureFile", "AttachPartIIOrder"}
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

                if (!await _pdfUpload.IsValidPdfFile(file))
                {
                    ModelState.AddModelError($"{formPrefix}.{fileProp.Value}", "File is not a valid PDF or appears to be a disguised file type.");
                    isValid = false;
                }

                if (await _pdfUpload.IsPdfPasswordProtected(file))
                {
                    ModelState.AddModelError($"{formPrefix}.{fileProp.Value}", "Password-protected PDFs are not allowed.");
                    isValid = false;
                }

                if (await _pdfUpload.ContainsMaliciousPdfContent(file))
                {
                    ModelState.AddModelError($"{formPrefix}.{fileProp.Value}", "PDF contains potentially malicious content.");
                    isValid = false;
                }
            }

            return isValid;
        }

        private async Task<ClaimCommonModel> SaveClaimCommonDataAsync(DTOClaimApplication model)
        {
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            if (model.ClaimCommonData == null) return new ClaimCommonModel();

            model.ClaimCommonData.ApplicantType = int.Parse(model.Category);
            model.ClaimCommonData.WithdrawPurpose = int.Parse(model.Purpose);
            model.ClaimCommonData.IOArmyNo = string.IsNullOrEmpty(model.COArmyNo) ? "" : model.COArmyNo;
            model.ClaimCommonData.IPAddress = ip;
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
        //public async Task<IActionResult> SubmitDocuments(ClaimFileUploadViewModel model, string formType, int applicationId)
        //{
        //    var files = new List<IFormFile>();
        //    if (model.CancelledCheque != null) files.Add(model.CancelledCheque);
        //    if (model.PaySlipPdf != null) files.Add(model.PaySlipPdf);            
        //    if (model.SpdocusPdf != null) files.Add(model.SpdocusPdf);
        //    if(model.SeviceExtnPdf != null) files.Add(model.SeviceExtnPdf);

        //    if (files.Count == 0)
        //    {
        //        ModelState.AddModelError("", "Please upload at least one file.");
        //        return View("Upload", model);
        //    }

        //    foreach (var file in files)
        //    {
        //        if (file.ContentType != "application/pdf")
        //        {
        //            ModelState.AddModelError(file.Name, "Only PDF files are allowed.");
        //        }

        //        if (file.Length > 150 * 1024)
        //        {
        //            ModelState.AddModelError(file.Name, "File size must not exceed 150 KB.");
        //            return View();
        //        }

        //        if (file.Length > 1 * 1024 * 1024)
        //        {
        //            ModelState.AddModelError(file.Name, "File size must not exceed 1 MB.");
        //        }

        //        if (!await _pdfUpload.IsValidPdfFile(file))
        //        {
        //            ModelState.AddModelError(file.Name, "File is not a valid PDF or appears to be a disguised file type.");
        //        }

        //        if (await _pdfUpload.IsPdfPasswordProtected(file))
        //        {
        //            ModelState.AddModelError(file.Name, "Password-protected PDF are not allowed.");
        //        }

        //        if (await _pdfUpload.ContainsMaliciousPdfContent(file))
        //        {
        //            ModelState.AddModelError(file.Name, "PDF contains potentially malicious content.");
        //        }
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return View("Upload", model);
        //    }



        //    bool success = await _IClaimonlineApplication1.ProcessFileUploads(files, formType, applicationId);


        //    if (!success)
        //    {
        //        ModelState.AddModelError("", "File upload failed. Please try again.");
        //        return View("Upload");
        //    }

        //    // Return a success message or redirect after successful upload
        //    return RedirectToAction("ApplicationDetails", "Claim");
        //}

        public async Task<IActionResult> Upload(ClaimFileUploadViewModel model, string formType, int applicationId)
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
                return View("Upload", model);
            }

            bool success = await _IClaimonlineApplication1.ProcessFileUploads(files, formType, applicationId);

            if (!success)
            {
                ModelState.AddModelError("", "File upload failed. Please try again.");
                return View("Upload", model);
            }

            // Return a success message or redirect after successful upload
            return RedirectToAction("ApplicationDetails", "Claim");
        }

        // Helper to gather uploaded files
        private List<IFormFile> GetUploadedFiles(ClaimFileUploadViewModel model)
        {
            var files = new List<IFormFile>();
            if (model.CancelledCheque != null) files.Add(model.CancelledCheque);
            if (model.PaySlipPdf != null) files.Add(model.PaySlipPdf);
            if (model.SpdocusPdf != null) files.Add(model.SpdocusPdf);
            if (model.SeviceExtnPdf != null) files.Add(model.SeviceExtnPdf);
            return files;
        }

        // Helper to validate a single file
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

            if (!await _pdfUpload.IsValidPdfFile(file))
            {
                ModelState.AddModelError(file.Name, "File is not a valid PDF or appears to be a disguised file type.");
            }

            if (await _pdfUpload.IsPdfPasswordProtected(file))
            {
                ModelState.AddModelError(file.Name, "Password-protected PDFs are not allowed.");
            }

            if (await _pdfUpload.ContainsMaliciousPdfContent(file))
            {
                ModelState.AddModelError(file.Name, "PDF contains potentially malicious content.");
            }
        }


        [HttpPost]
        public async Task<JsonResult> MergePdf(int applicationId, bool isRejected, bool isApproved)
        {
           
            try
            {
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (string.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Connection.RemoteIpAddress?.ToString();
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
                    else if(applicationType == "4")
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


                // Check if source folder exists
                if (!Directory.Exists(sourceFolderPath))
                {
                    return Json(new { success = false, message = $"Source folder not found: {sourceFolderPath}" });
                }

                // Get all PDF files from the source folder
                string[] pdfFiles = Directory.GetFiles(sourceFolderPath, "*.pdf");

                if (pdfFiles.Length == 0)
                {
                    return Json(new { success = false, message = "No PDF files found in the specified folder." });
                }

                // Generate the new PDF first (if needed)
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
                        pdfFiles = Directory.GetFiles(sourceFolderPath, "*.pdf").OrderBy(file => Path.GetFileName(file))
                                 .ToArray(); ;
                    }
                }
                catch (Exception pdfGenEx)
                {
                    Console.WriteLine($"Error generating PDF: {pdfGenEx.Message}");
                    // Continue with existing PDFs if generation fails
                }

                // Create merged PDF path in TempUploads root
                string tempUploadsPath = Path.Combine(_env.WebRootPath, "ClaimMergePdf");
                if (!Directory.Exists(tempUploadsPath))
                {
                    Directory.CreateDirectory(tempUploadsPath);
                }

                string MergePdfName = "App" + applicationIdStr + armyNo;
                string mergedPdfPath = Path.Combine(tempUploadsPath, MergePdfName + ".pdf");
                ViewBag.MergedPdfPath = mergedPdfPath;
                // Merge all PDFs using iText7
                bool mergeResult = await _mergePdf.MergePdfFiles(pdfFiles, mergedPdfPath);

                ReaderProperties readerProperties = new ReaderProperties();
                PdfReader pdfReader = new PdfReader(mergedPdfPath, readerProperties);
                _watermark.OpenPdf(pdfReader, ip, mergedPdfPath);

                if (mergeResult)
                {
                    // Get relative path for client
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
                return Json(new { success = false, message = $"Error occurred while merging PDFs: {ex.Message}" });
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

                // Create a response object with the required fields for the modal
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
                // Log the exception if you have logging setup
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
                // Call the FileUtility method to save the Base64 string to a file
                await _fileUtility.SaveBase64ToFileAsync(base64String, directoryPath, fileName);
                return Json(new { success = true, message = "File saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error saving file: {ex.Message}" });
            }
        }
    }
}
