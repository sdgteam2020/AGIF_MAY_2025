
using Agif_V2.Helpers;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Agif_V2.Controllers
{
    public class ClaimController : Controller
    {
        private readonly IClaimOnlineApplication _IClaimonlineApplication1;
        private readonly IMasterOnlyTable _IMasterOnlyTable;
        private readonly PdfGenerator _pdfGenerator;
        private readonly MergePdf _mergePdf;
        private readonly IWebHostEnvironment _env;
        private readonly ICar _car;
        private readonly IHba _Hba;
        private readonly IPca _Pca;

        public ClaimController(IClaimOnlineApplication OnlineApplication, IMasterOnlyTable MasterOnlyTable, ICar _car, IHba _Hba, IPca _Pca, PdfGenerator pdfGenerator, IWebHostEnvironment env, MergePdf mergePdf)
        {
            _IClaimonlineApplication1 = OnlineApplication;
            _IMasterOnlyTable = MasterOnlyTable;
            this._car = _car;
            this._Hba = _Hba;
            this._Pca = _Pca;
            _pdfGenerator = pdfGenerator;
            _env = env;
            _mergePdf = mergePdf;
        }

        public IActionResult MaturityLoanType()
        {
            return View();
        }

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
            // Handle the received loanType and applicantCategory
            // You can now access the form data here
            // For example, you can pass these values to a view or use them in processing logic
            string AppCategory = EncryptDecrypt.EncryptionData(Category);
            string WithdrwalPurpose = EncryptDecrypt.EncryptionData(PurposeOfWithdrwal);

            TempData["Category"] = AppCategory;
            TempData["WithdrwalPurpose"] = WithdrwalPurpose;
            return RedirectToAction("OnlineApplication");
        }


        public async Task<IActionResult> SubmitApplication(DTOClaimApplication model)
        {
            string formType = string.Empty;
            if (model.EducationDetails != null)
            {
                var educationContext = new ValidationContext(model.EducationDetails);
                var educationValidationResults = new List<ValidationResult>();

                if (!Validator.TryValidateObject(model.EducationDetails, educationContext, educationValidationResults, true))
                {
                    foreach (var result in educationValidationResults)
                    {
                        string propertyName = result.MemberNames?.FirstOrDefault();
                        string errorKey = string.IsNullOrEmpty(propertyName)
                            ? "EducationDetails"
                            : $"EducationDetails.{propertyName}";
                        ModelState.AddModelError(errorKey, result.ErrorMessage);
                    }
                }
                // File Upload Validation for EducationDetails

                if (model.EducationDetails.AttachPartIIOrder != null)
                {
                    string errorMessage;
                    if (!_IClaimonlineApplication1.ValidateFileUpload(model.EducationDetails.AttachPartIIOrder, out errorMessage))
                    {
                        ModelState.AddModelError("EducationDetails.AttachPartIIOrder", errorMessage);
                    }
                }

                if (model.EducationDetails.AttachBonafideLetter != null)
                {
                    string errorMessage;
                    if (!_IClaimonlineApplication1.ValidateFileUpload(model.EducationDetails.AttachBonafideLetter, out errorMessage))
                    {
                        ModelState.AddModelError("EducationDetails.AttachInvitationcard", errorMessage);
                    }
                }
            }
            else if (model.Marriageward != null)
            {
                var pcaValidationContext = new ValidationContext(model.Marriageward);
                var pcaValidationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(model.Marriageward, pcaValidationContext, pcaValidationResults, true))
                {
                    foreach (var result in pcaValidationResults)
                    {
                        string propertyName = result.MemberNames?.FirstOrDefault();
                        string errorKey = string.IsNullOrEmpty(propertyName)
                            ? "Marriageward"
                            : $"Marriageward.{propertyName}";
                        ModelState.AddModelError(errorKey, result.ErrorMessage);
                    }

                }
                if (model.Marriageward.AttachPartIIOrder != null)
                {
                    string errorMessage;
                    if (!_IClaimonlineApplication1.ValidateFileUpload(model.Marriageward.AttachPartIIOrder, out errorMessage))
                    {
                        ModelState.AddModelError("EducationDetails.AttachPartIIOrder", errorMessage);
                    }
                }

                if (model.Marriageward.AttachInvitationcard != null)
                {
                    string errorMessage;
                    if (!_IClaimonlineApplication1.ValidateFileUpload(model.Marriageward.AttachInvitationcard, out errorMessage))
                    {
                        ModelState.AddModelError("EducationDetails.AttachInvitationcard", errorMessage);
                    }
                }
            }
            else if (model.PropertyRenovation != null)
            {
                var hbaValidationContext = new ValidationContext(model.PropertyRenovation);
                var hbaValidationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(model.PropertyRenovation, hbaValidationContext, hbaValidationResults, true))
                {
                    foreach (var result in hbaValidationResults)
                    {
                        string propertyName = result.MemberNames?.FirstOrDefault();
                        string errorKey = string.IsNullOrEmpty(propertyName)
                            ? "PropertyRenovation"
                            : $"PropertyRenovation.{propertyName}";
                        ModelState.AddModelError(errorKey, result.ErrorMessage);
                    }
                }

                if (model.PropertyRenovation.TotalExpenditureFile != null)
                {
                    string errorMessage;
                    if (!_IClaimonlineApplication1.ValidateFileUpload(model.PropertyRenovation.TotalExpenditureFile, out errorMessage))
                    {
                        ModelState.AddModelError("EducationDetails.AttachPartIIOrder", errorMessage);
                    }
                }
            }
            else if (model.SplWaiver != null)
            {
                var splWaiverValidationContext = new ValidationContext(model.SplWaiver);
                var splWaiverValidationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(model.SplWaiver, splWaiverValidationContext, splWaiverValidationResults, true))
                {
                    foreach (var result in splWaiverValidationResults)
                    {
                        string propertyName = result.MemberNames?.FirstOrDefault();
                        string errorKey = string.IsNullOrEmpty(propertyName)
                            ? "SplWaiver"
                            : $"SplWaiver.{propertyName}";
                        ModelState.AddModelError(errorKey, result.ErrorMessage);
                    }
                }
            }



            // Also validate the CommonData if it exists
            if (model.ClaimCommonData != null)
            {
                var commonDataValidationContext = new ValidationContext(model.ClaimCommonData);
                var commonDataValidationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(model.ClaimCommonData, commonDataValidationContext, commonDataValidationResults, true))
                {
                    foreach (var result in commonDataValidationResults)
                    {
                        string propertyName = result.MemberNames?.FirstOrDefault();
                        string errorKey = string.IsNullOrEmpty(propertyName)
                            ? "ClaimCommonData"
                            : $"ClaimCommonData.{propertyName}";
                        ModelState.AddModelError(errorKey, result.ErrorMessage);
                    }
                }
            }

            if (!ModelState.IsValid)
                return View("OnlineApplication", model);
            else
            {
                ClaimCommonModel claimCommonModel = new ClaimCommonModel();
                if (model.ClaimCommonData != null)
                {
                    model.ClaimCommonData.ApplicantType = int.Parse(model.Category);
                    model.ClaimCommonData.WithdrawPurpose = int.Parse(model.Purpose);

                    //claimCommonModel = await _IClaimonlineApplication1.AddWithReturn(model.ClaimCommonData);
                }

                if (model.EducationDetails != null)
                {
                    formType = "ED";
                    bool result = await _IClaimonlineApplication1.submitApplication(model, "ED", claimCommonModel.ApplicationId);
                   // bool result = await _IClaimonlineApplication1.submitApplication(model, "ED", 1002);

                }
                else if (model.Marriageward != null)
                {
                    formType = "MW";
                    bool result = await _IClaimonlineApplication1.submitApplication(model, "MW", claimCommonModel.ApplicationId);
                }
                else if (model.PropertyRenovation != null)
                {
                    formType = "PR";
                    bool result = await _IClaimonlineApplication1.submitApplication(model, "PR", claimCommonModel.ApplicationId);
                }
                else
                {
                    formType = "SP";
                    bool result = await _IClaimonlineApplication1.submitApplication(model, "SP", claimCommonModel.ApplicationId);

                }

                TempData["applicationId"] = claimCommonModel.ApplicationId;

                return RedirectToAction("Upload", "Claim", new { formType });

            }



        }

        public async Task<IActionResult> Upload()
        {
            ClaimFileUploadViewModel ClaimfileUploadViewModel = new ClaimFileUploadViewModel();
            return View(ClaimfileUploadViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitDocuments(ClaimFileUploadViewModel model, string formType, int applicationId)
        {
            var files = new List<IFormFile>();
            if (model.CancelledCheque != null) files.Add(model.CancelledCheque);
            if (model.PaySlipPdf != null) files.Add(model.PaySlipPdf);            
            if (model.Spdocus != null) files.Add(model.Spdocus);

            if (files.Count == 0)
            {
                ModelState.AddModelError("", "Please upload at least one file.");
                return View("Upload", model);
            }

            foreach (var file in files)
            {
                if (file.ContentType != "application/pdf")
                {
                    ModelState.AddModelError(file.Name, "Only PDF files are allowed.");
                }
                if (file.Length > 1 * 1024 * 1024)
                {
                    ModelState.AddModelError(file.Name, "File size must not exceed 1 MB.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("Upload", model);
            }

            bool success = await _IClaimonlineApplication1.ProcessFileUploads(files, formType, applicationId);


            if (!success)
            {
                ModelState.AddModelError("", "File upload failed. Please try again.");
                return View("Upload");
            }

            // Return a success message or redirect after successful upload
            return RedirectToAction("Upload");
        }
    }
}
