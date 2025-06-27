using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.StyledXmlParser.Node;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Agif_V2.Controllers
{
    public class UploadController : Controller
    {
        private readonly IOnlineApplication _IonlineApplication1;
        private readonly IArmyPrefixes _IArmyPrefixes;
       private readonly IDoucmentupload _IDocumentUpload;
        public UploadController(IOnlineApplication OnlineApplication, IArmyPrefixes _IArmyPrefixes, IDoucmentupload _IDocumentUpload)
        {
            _IonlineApplication1 = OnlineApplication;
            this._IArmyPrefixes = _IArmyPrefixes;
            this._IDocumentUpload = _IDocumentUpload;
        }
        public IActionResult Upload()
        {
           
            FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
            return View(fileUploadViewModel);
        }

        public async Task<IActionResult> ApplicationDetails(int applicationId)
        {
            if (applicationId == 0)
            {
                return NotFound();
            }

            var application = await _IonlineApplication1.GetApplicationDetails(applicationId);
            if (application == null)
            {
                return NotFound();
            }

            ViewBag.Message = TempData["Message"];

            return View(application);
        }

        // Update ProcessUpload to redirect to ApplicationDetails after upload
        [HttpPost]
        public async Task<IActionResult> ProcessUpload(FileUploadViewModel model, string formType, int applicationId)
        {
            CommonDataModel commonDataModel = new CommonDataModel();
            MArmyPrefix mArmyPrefix = new MArmyPrefix();
            string ArmyNo = string.Empty;

            string CoArmyNumber= TempData["COArmyNumber"] as string ?? string.Empty;

            if (applicationId != 0)
            {
                commonDataModel = await _IonlineApplication1.Get(applicationId);
                int id = commonDataModel.ArmyPrefix;
                mArmyPrefix = await _IArmyPrefixes.Get(id);
                ArmyNo = (mArmyPrefix.Prefix ?? "") + (commonDataModel.Number ?? "") + (commonDataModel.Suffix ?? "");
                ArmyNo = ArmyNo.Trim();
            }

            var files = new List<IFormFile>();
            if (model.CancelledCheque != null) files.Add(model.CancelledCheque);
            if (model.PaySlipPdf != null) files.Add(model.PaySlipPdf);
            if (model.QuotationPdf != null) files.Add(model.QuotationPdf);
            if (model.DrivingLicensePdf != null) files.Add(model.DrivingLicensePdf);
            if (model.SeviceExtnPdf != null) files.Add(model.SeviceExtnPdf);

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

            ViewData["formType"] = formType;

            if (!ModelState.IsValid)
            {
                return View("Upload", model);
            }


            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempUploads");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            string folderName = $"{formType}_{ArmyNo}_{applicationId}";
            string folderPath = Path.Combine(tempFolder, folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileUpload = new DocumentUpload();
            fileUpload.ApplicationId = applicationId;

            foreach (var file in files)
            {
                string fileExtension = Path.GetExtension(file.FileName);
                string fileName = $"{formType}_{ArmyNo}_{applicationId}_{file.Name}{fileExtension}";
                string outputFile = Path.Combine(folderPath, fileName);

                using (var fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                if (file.Name == nameof(model.CancelledCheque))
                {
                    fileUpload.CancelledCheque = fileName;
                    fileUpload.IsCancelledCheque = true;
                }
                else if (file.Name == nameof(model.PaySlipPdf))
                {
                    fileUpload.PaySlipPdf = fileName;
                    fileUpload.IsPaySlipPdf = true;
                }
                else if (file.Name == nameof(model.QuotationPdf))
                {
                    fileUpload.QuotationPdf = fileName;
                    fileUpload.IsQuotationPdf = true;
                }
                else if (file.Name == nameof(model.DrivingLicensePdf))
                {
                    fileUpload.DrivingLicensePdf = fileName;
                    fileUpload.IsDrivingLicensePdf = true;
                }
                else if (file.Name == nameof(model.SeviceExtnPdf))
                {
                    fileUpload.SeviceExtnPdf = fileName;
                    fileUpload.IsSeviceExtnPdf = true;
                }
            }

            await _IDocumentUpload.Add(fileUpload);

            await _IonlineApplication1.UpdateApplicationStatus(applicationId, 1);

            var IOArmyNo = await _IonlineApplication1.GetIOArmyNoAsync(applicationId);
            if (IOArmyNo == null)
            {
                if (!string.IsNullOrEmpty(CoArmyNumber))
                {
                    var CoDetails = await _IonlineApplication1.GetUserDetails(CoArmyNumber);
                    if (CoDetails != null)
                    {
                        TrnFwdCO trnFwdCO = new TrnFwdCO
                        {
                            ApplicationId = applicationId,
                            ArmyNo = ArmyNo,
                            COUserId = CoDetails.UserId,
                            ProfileId = CoDetails.ProfileId,
                            CreatedOn = DateTime.Now,
                            Status = 1
                        };
                        await _IonlineApplication1.AddFwdCO(trnFwdCO);
                    }

                }
            }
            else
            {
                if (!string.IsNullOrEmpty(IOArmyNo))
                {
                    var IoDetails = await _IonlineApplication1.GetUserDetails(IOArmyNo);
                    if (IoDetails != null)
                    {
                        TrnFwdCO trnFwdCO = new TrnFwdCO
                        {
                            ApplicationId = applicationId,
                            ArmyNo = ArmyNo,
                            COUserId = IoDetails.UserId,
                            ProfileId = IoDetails.ProfileId,
                            CreatedOn = DateTime.Now,
                            Status = 1
                        };
                        await _IonlineApplication1.AddFwdCO(trnFwdCO);
                    }

                }
            }

            //if (!string.IsNullOrEmpty(CoArmyNumber))
            //{
            //    var CoDetails = await _IonlineApplication1.GetUserDetails(CoArmyNumber);
            //    if (CoDetails != null)
            //    {
            //        TrnFwdCO trnFwdCO = new TrnFwdCO
            //        {
            //            ApplicationId = applicationId,
            //            ArmyNo = ArmyNo,
            //            COUserId = CoDetails.UserId,
            //            ProfileId = CoDetails.ProfileId,
            //            CreatedOn = DateTime.Now,
            //            Status = 1
            //        };
            //        await _IonlineApplication1.AddFwdCO(trnFwdCO);
            //    }

            //}
            
            TempData["Message"] = "Application is forwarded to your Unit Cdr.";
            return RedirectToAction("ApplicationDetails", new { applicationId = applicationId});
        }
        public IActionResult UploadSuccess()
        {
            return View();
        }

        [HttpPost]
        public JsonResult InfoBeforeUpload(string applicationId)
        {
            if (string.IsNullOrWhiteSpace(applicationId) || applicationId == "0")
            {
                return Json(new { success = false, message = "Application ID is required." });
            }

            var coDetails = _IonlineApplication1.GetUnitByApplicationId(int.Parse(applicationId));
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


    }
}
