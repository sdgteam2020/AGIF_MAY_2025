using Agif_V2.Helpers;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Agif_V2.Controllers
{
    public class OnlineApplicationController : Controller
    {
        private readonly IOnlineApplication _IonlineApplication1;
        private readonly IMasterOnlyTable _IMasterOnlyTable;
        private readonly PdfGenerator _pdfGenerator;
        private readonly MergePdf _mergePdf;
        private readonly IWebHostEnvironment _env;
        private readonly ICar _car;
        private readonly IHba _Hba;
        private readonly IPca _Pca;

        public OnlineApplicationController(IOnlineApplication OnlineApplication, IMasterOnlyTable MasterOnlyTable, ICar _car, IHba _Hba, IPca _Pca, PdfGenerator pdfGenerator, IWebHostEnvironment env, MergePdf mergePdf)
        {
            _IonlineApplication1 = OnlineApplication;
            _IMasterOnlyTable = MasterOnlyTable;
            this._car = _car;
            this._Hba = _Hba;
            this._Pca = _Pca;
            _pdfGenerator = pdfGenerator;
            _env = env;
            _mergePdf = mergePdf;
        }

        public async Task<IActionResult> OnlineApplication()
        {
            var loanType = TempData["LoanType"] as string;
            var applicantCategory = TempData["ApplicantCategory"] as string;


            TempData["loantypeNew"] = EncryptDecrypt.DecryptionData(loanType);

            TempData["applicantcategoryNew"] = EncryptDecrypt.DecryptionData(applicantCategory);


            TempData.Keep("LoanType");
            TempData.Keep("ApplicantCategory");

            DTOOnlineApplication DTOOnlineapplication = new DTOOnlineApplication();
            return View(DTOOnlineapplication);
        }
        //public IActionResult SaveApplication(DTOOnlineApplicationRequest Data)
        //{
        //    var validationContext = new ValidationContext(Data.onlineApplications);
        //    var validationResults = new List<ValidationResult>();

        //    bool isValid = Validator.TryValidateObject(
        //        Data.onlineApplications,
        //        validationContext,
        //        validationResults,
        //        validateAllProperties: true);

        //    if (!isValid)
        //    {
        //        // validationResults contains errors
        //    }
        //    return View();  
        //}
        public IActionResult LoanType()
        {
            return View();
        }
        public async Task<JsonResult> GetRetirementDate(int rankId, int Prefix,int regtId)
        {
            var userType = await _IMasterOnlyTable.GetUserType(Prefix);
            var retAge = await _IMasterOnlyTable.GetRetirementAge(rankId, regtId);
            var retirementAge = retAge.FirstOrDefault()?.RetirementAge ?? 0;
            var userTypeId = userType.FirstOrDefault()?.UserType ?? 0;
            if (retirementAge > 0 && userTypeId != 0)
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
            if (!string.IsNullOrEmpty(pcdaPao))
            {
                return Json(new { pcdaPao = pcdaPao });
            }
            else
            {
                return Json("0");
            }
        }

        public async Task<JsonResult> CheckExistUser(string armyNumber, string Prefix, string Suffix, int appType)
        {
            var existingUser = await _IonlineApplication1.GetApplicationDetailsByArmyNo(armyNumber, Prefix, Suffix, appType);

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
            bool result = await _IonlineApplication1.DeleteExistingLoan(armyNumber, Prefix, Suffix, appType);

            if (result == true) 
            {
                return Json(new { exists = true }); 
            }
            else
            {
                return Json(new { exists = false }); 
            }
        }

        public IActionResult Redirection(string loanType, string applicantCategory)
        {
            // Handle the received loanType and applicantCategory
            // You can now access the form data here
            // For example, you can pass these values to a view or use them in processing logic
            string Loan = EncryptDecrypt.EncryptionData(loanType);
            string Category = EncryptDecrypt.EncryptionData(applicantCategory);

            TempData["LoanType"] = Loan;
            TempData["ApplicantCategory"] = Category;
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
            return Json(await _IonlineApplication1.CheckIsCoRegister(UnitId));
        }



        /*
        public IActionResult SubmitApplication(DTOOnlineApplication model)
        {
            //string formType = string.Empty;

            //// Check which application model is populated
            //if (model.CarApplication != null)
            //{
            //    formType = "CA"; // If CarApplication is populated, use "CA"
            //}
            //else if (model.PCAApplication != null)
            //{
            //    formType = "PCA"; // If PCAApplication is populated, use "PCA"
            //}
            //else if (model.HBAApplication != null)
            //{
            //    formType = "HBA"; // If HBAApplication is populated, use "HBA"
            //}
            //else
            //{
            //    // Handle the case where none of the models are populated (optional)
            //    return RedirectToAction("OnlineApplication", "OnlineApplication"); // Or any appropriate action
            //}

            //if (!ModelState.IsValid)
            //{
            //    return View("OnlineApplication", model); // Redirect to OnlineApplication if validation fails
            //}

            //// Redirect to the Upload action with the formType
            //return RedirectToAction("Upload", "Upload", new { formType });

            string formType = string.Empty;

            // Perform server-side validation on the entire model
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Validate the DTO model and all its properties (including nested models)
            bool isValid = Validator.TryValidateObject(
                model,
                validationContext,
                validationResults,
                validateAllProperties: true
            );

            // If validation fails, add errors to ModelState and return the view with validation errors
            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    // Add each validation error message to ModelState
                    ModelState.AddModelError("", validationResult.ErrorMessage);
                }

                // Return to the same view with validation errors
                return View("OnlineApplication", model);
            }

            // If the model is valid, check which application model is populated and set formType
            if (model.CarApplication != null)
            {
                formType = "CA"; // If CarApplication is populated, use "CA"
            }
            else if (model.PCAApplication != null)
            {
                formType = "PCA"; // If PCAApplication is populated, use "PCA"
            }
            else if (model.HBAApplication != null)
            {
                formType = "HBA"; // If HBAApplication is populated, use "HBA"
            }
            else
            {
                ModelState.AddModelError("", "Please select an application type."); // Add validation error
                return View("OnlineApplication", model); // Return the view with validation errors
            }

            // Proceed to the next step (e.g., redirecting to Upload page)
            return RedirectToAction("Upload", "Upload", new { formType });

        }
        */
        public async Task<IActionResult> SubmitApplication(DTOOnlineApplication model)
            {
            string formType = string.Empty;


            // First, determine the form type
            if (model.CarApplication != null)
            {
                formType = "CA";
            }
            else if (model.PCAApplication != null)
            {
                formType = "PCA";
            }
            else if (model.HBAApplication != null)
            {
                formType = "HBA";
            }
            else
            {
                ModelState.AddModelError("", "Please select an application type.");
            }

            // Validate nested objects more specifically based on form type
            if (formType == "CA" && model.CarApplication != null)
            {
                var carValidationContext = new ValidationContext(model.CarApplication);
                var carValidationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(model.CarApplication, carValidationContext, carValidationResults, true))
                {
                    foreach (var result in carValidationResults)
                    {
                        string propertyName = result.MemberNames?.FirstOrDefault();
                        string errorKey = string.IsNullOrEmpty(propertyName)
                            ? "CarApplication"
                            : $"CarApplication.{propertyName}";
                        ModelState.AddModelError(errorKey, result.ErrorMessage);
                    }
                }
            }
            else if (formType == "PCA" && model.PCAApplication != null)
            {
                var pcaValidationContext = new ValidationContext(model.PCAApplication);
                var pcaValidationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(model.PCAApplication, pcaValidationContext, pcaValidationResults, true))
                {
                    foreach (var result in pcaValidationResults)
                    {
                        string propertyName = result.MemberNames?.FirstOrDefault();
                        string errorKey = string.IsNullOrEmpty(propertyName)
                            ? "PCAApplication"
                            : $"PCAApplication.{propertyName}";
                        ModelState.AddModelError(errorKey, result.ErrorMessage);
                    }
                }
            }
            else if (formType == "HBA" && model.HBAApplication != null)
            {
                var hbaValidationContext = new ValidationContext(model.HBAApplication);
                var hbaValidationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(model.HBAApplication, hbaValidationContext, hbaValidationResults, true))
                {
                    foreach (var result in hbaValidationResults)
                    {
                        string propertyName = result.MemberNames?.FirstOrDefault();
                        string errorKey = string.IsNullOrEmpty(propertyName)
                            ? "HBAApplication"
                            : $"HBAApplication.{propertyName}";
                        ModelState.AddModelError(errorKey, result.ErrorMessage);
                    }
                }
            }

            // Also validate the CommonData if it exists
            if (model.CommonData != null)
            {
                var commonDataValidationContext = new ValidationContext(model.CommonData);
                var commonDataValidationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(model.CommonData, commonDataValidationContext, commonDataValidationResults, true))
                {
                    foreach (var result in commonDataValidationResults)
                    {
                        string propertyName = result.MemberNames?.FirstOrDefault();
                        string errorKey = string.IsNullOrEmpty(propertyName)
                            ? "CommonData"
                            : $"CommonData.{propertyName}";
                        ModelState.AddModelError(errorKey, result.ErrorMessage);
                    }
                }
            }

            // int Applicationtype = int.Parse(model.loantype);
            //int ApplicantType = int.Parse(model.applicantCategory);
            // Check ModelState validity after all validations
            if (!ModelState.IsValid)
            {

                // Preserve the loan type for the view
                return View("OnlineApplication", model);
            }


            else
            {
                CommonDataModel common = new CommonDataModel();

                try
                {

                    if (model.CommonData != null)
                    {
                        model.CommonData.ApplicationType = int.Parse(model.loantype);
                        model.CommonData.ApplicantType = int.Parse(model.applicantCategory);
                        model.CommonData.IOArmyNo = string.IsNullOrEmpty(model.COArmyNo) ? "" : model.COArmyNo;
                        common = await _IonlineApplication1.AddWithReturn(model.CommonData);
                    }

                    if (formType == "HBA" && model.HBAApplication != null)
                    {
                        HBAApplicationModel HBA = new HBAApplicationModel();

                        HBA = model.HBAApplication;

                        HBA.ApplicationId = common.ApplicationId;

                        await _Hba.Add(HBA);
                    }

                    else if (formType == "CA" && model.CarApplication != null)
                    {
                        CarApplicationModel Car = new CarApplicationModel();

                        Car = model.CarApplication;

                        Car.ApplicationId = common.ApplicationId;

                        await _car.Add(Car);
                    }

                    else if (formType == "PCA" && model.PCAApplication != null)
                    {
                        PCAApplicationModel PCA = new PCAApplicationModel();

                        PCA = model.PCAApplication;

                        PCA.ApplicationId = common.ApplicationId;

                        await _Pca.Add(PCA);
                    }

                }

                catch (Exception ex)
                {

                    ModelState.AddModelError("", "An error occurred while processing your application.");
                }

                // TempData["Message"] = "Your application has been saved successfully. Please upload the required document to proceed.";

                //int Applicationid = common.ApplicationId;

               TempData["applicationId"] = common.ApplicationId;
                //TempData["applicationId"] = 2039; // For testing purposes, replace with actual ApplicationId from common.ApplicationId
                TempData["Message"] = "Your application has been saved successfully. Please upload the required document to proceed.";

                TempData["COArmyNumber"] = model.COArmyNo;
                return RedirectToAction("Upload", "Upload", new { formType });





                // Proceed to the next step
            }

        }

        public async Task<IActionResult> GetApplicationDetails(int applicationId)
        {
            return View();
        }

        public async Task<JsonResult> MergePdf(int applicationId,bool isRejected,bool isApproved)
        {
            try
            {
                string ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (string.IsNullOrEmpty(ip))
                {
                    ip = HttpContext.Connection.RemoteIpAddress?.ToString();
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
                        applicationTypeName = "CA";
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
                string folderPath = applicationTypeName + "_" + armyNo + "_" + applicationIdStr;
                string sourceFolderPath = Path.Combine(_env.WebRootPath, "TempUploads", folderPath);


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

                    string Name = await _IonlineApplication1.GetCOName(dTOTempSession.ProfileId);

                    var data = await _pdfGenerator.CreatePdfForOnlineApplication(applicationId, generatedPdfPath,isRejected,isApproved,dTOTempSession.UserName,ip, Name);

                    if (data == 1)
                    {
                        pdfFiles = Directory.GetFiles(sourceFolderPath, "*.pdf").OrderBy(file => Path.GetFileName(file))
                                 .ToArray(); ;
                       // pdfFiles.OrderByDescending().ToArray(); // Ensure the latest PDF is included
                    }
                }
                catch (Exception pdfGenEx)
                {
                    Console.WriteLine($"Error generating PDF: {pdfGenEx.Message}");
                    // Continue with existing PDFs if generation fails
                }

                // Create merged PDF path in TempUploads root
                string tempUploadsPath = Path.Combine(_env.WebRootPath, "TempUploads", folderPath);
                if (!Directory.Exists(tempUploadsPath))
                {
                    Directory.CreateDirectory(tempUploadsPath);
                }

                string mergedPdfPath = Path.Combine(tempUploadsPath, folderPath + "_Merged.pdf");
                ViewBag.MergedPdfPath = mergedPdfPath;
                // Merge all PDFs using iText7
                bool mergeResult = await _mergePdf.MergePdfFiles(pdfFiles, mergedPdfPath);

                if (mergeResult)
                {
                    // Get relative path for client
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
                    applicationTypeName = "CA";
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
            string folderPath = applicationTypeName + "_" + armyNo + "_" + applicationIdStr;
            string pdfFilePath = $"/TempUploads/{folderPath}/{folderPath}_Merged.pdf";
            
            return Json(pdfFilePath);
        }
    }

}