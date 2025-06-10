using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using DataTransferObject.Request;
using Microsoft.AspNetCore.Builder;
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
        private readonly ICar _car;
        private readonly IHba _Hba;
        private readonly IPca _Pca;

        public OnlineApplicationController(IOnlineApplication OnlineApplication, IMasterOnlyTable MasterOnlyTable, ICar _car, IHba _Hba, IPca _Pca)
        {
            _IonlineApplication1 = OnlineApplication;
            _IMasterOnlyTable = MasterOnlyTable;
            this._car=_car;
            this._Hba = _Hba;
            this._Pca = _Pca;
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

        public IActionResult Redirection(string loanType, string applicantCategory)
        {
            // Handle the received loanType and applicantCategory
            // You can now access the form data here
            // For example, you can pass these values to a view or use them in processing logic
            string Loan= EncryptDecrypt.EncryptionData(loanType);
            string Category = EncryptDecrypt.EncryptionData(applicantCategory);

            TempData["LoanType"] = Loan;
            TempData["ApplicantCategory"] = Category;
            return RedirectToAction("OnlineApplication");
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

                catch(Exception ex)
                {

                    ModelState.AddModelError("", "An error occurred while processing your application.");
                }
               // TempData["Message"] = "Your application has been saved successfully. Please upload the required document to proceed.";

                int Applicationid = common.ApplicationId;
                TempData["applicationId"] = Applicationid;
                TempData["Message"] = "Your application has been saved successfully. Please upload the required document to proceed.";
                return RedirectToAction("Upload", "Upload", new { formType });

            }
            
           

            // Proceed to the next step
        }
    }
}
