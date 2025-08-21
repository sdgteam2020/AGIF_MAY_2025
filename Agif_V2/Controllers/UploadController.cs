using Agif_V2.Helpers;
using DataAccessLayer.Interfaces;
using DataTransferObject.Helpers;
using DataTransferObject.Model;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.StyledXmlParser.Node;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Agif_V2.Controllers
{
    public class UploadController : Controller
    {
        private readonly IOnlineApplication _IonlineApplication1;
        private readonly IArmyPrefixes _IArmyPrefixes;
       private readonly IDoucmentupload _IDocumentUpload;
        private readonly PdfUpload _pdfUpload;
        public UploadController(IOnlineApplication OnlineApplication, IArmyPrefixes _IArmyPrefixes, IDoucmentupload _IDocumentUpload,PdfUpload _pdfUpload)
        {
            _IonlineApplication1 = OnlineApplication;
            this._IArmyPrefixes = _IArmyPrefixes;
            this._IDocumentUpload = _IDocumentUpload;
            this._pdfUpload = _pdfUpload;
        }
        public async Task<IActionResult> Upload()
        {
            int applicationId = Convert.ToInt32(TempData["applicationId"]);

            bool application = await _IonlineApplication1.CheckDocumentUploaded(applicationId);

            string FormType = await _IonlineApplication1.GetFormType(applicationId);

            TempData.Keep("applicationId");

            if (application)
            {
                TempData["Message"] = "You have already uploaded the Documents for this Application.";
                return RedirectToAction("ApplicationDetails","Upload");
            }

            bool IsextensionOfService = await _IonlineApplication1.CheckExtensionofservice(applicationId);

            TempData["IsextensionOfService"] = IsextensionOfService;

            FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
            fileUploadViewModel.FormType= FormType;
            return View(fileUploadViewModel);
        }

        public async Task<IActionResult> ApplicationDetails()
        {
            int applicationId = Convert.ToInt32(TempData["applicationId"]);

            TempData.Keep("applicationId");

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
        //public async Task<IActionResult> ProcessUpload(FileUploadViewModel model, string formType, int applicationId)
        //{
        //    CommonDataModel commonDataModel = new CommonDataModel();
        //    MArmyPrefix mArmyPrefix = new MArmyPrefix();
        //    string ArmyNo = string.Empty;

        //    string CoArmyNumber= TempData["COArmyNumber"] as string ?? string.Empty;

        //    if (applicationId != 0)
        //    {
        //        commonDataModel = await _IonlineApplication1.Get(applicationId);

        //        int id = commonDataModel.ArmyPrefix;
        //        mArmyPrefix = await _IArmyPrefixes.Get(id);
        //        ArmyNo = (mArmyPrefix.Prefix ?? "") + (commonDataModel.Number ?? "") + (commonDataModel.Suffix ?? "");
        //        ArmyNo = ArmyNo.Trim();
        //    }

        //    if(formType == "CA")
        //    {
        //        int vehType = await _IonlineApplication1.GetVehicleType(applicationId, formType);
        //        formType = "CAR";
        //        if (vehType == 4)
        //        {
        //            formType = "TW";
        //        }
        //    }

        //    var files = new List<IFormFile>();
        //    if (model.CancelledCheque != null) files.Add(model.CancelledCheque);
        //    if (model.PaySlipPdf != null) files.Add(model.PaySlipPdf);
        //    if (model.QuotationPdf != null) files.Add(model.QuotationPdf);
        //    if (model.DrivingLicensePdf != null) files.Add(model.DrivingLicensePdf);
        //    if (model.SeviceExtnPdf != null) files.Add(model.SeviceExtnPdf);

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

        //        if(!await _pdfUpload.IsValidPdfFile(file))
        //        {
        //             ModelState.AddModelError(file.Name,"File is not a valid PDF or appears to be a disguised file type.");
        //        }

        //        if(await _pdfUpload.IsPdfPasswordProtected(file))
        //        {
        //             ModelState.AddModelError(file.Name, "Password-protected PDF are not allowed.");
        //        }

        //        if(await _pdfUpload.ContainsMaliciousPdfContent(file))
        //        {
        //             ModelState.AddModelError(file.Name, "PDF contains potentially malicious content.");
        //        }

        //    }

        //    //ViewData["formType"] = formType;

        //    if (!ModelState.IsValid)
        //    {
        //        return View("Upload", model);
        //    }


        //    string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempUploads");
        //    if (!Directory.Exists(tempFolder))
        //    {
        //        Directory.CreateDirectory(tempFolder);
        //    }

        //    string folderName = $"{formType}{ArmyNo}_{applicationId}";
        //    string folderPath = Path.Combine(tempFolder, folderName);
        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    var fileUpload = new DocumentUpload();
        //    fileUpload.ApplicationId = applicationId;

        //    foreach (var file in files)
        //    {
        //        string fileExtension = Path.GetExtension(file.FileName);
        //        string fileName = $"{file.Name}{fileExtension}";
        //        string outputFile = Path.Combine(folderPath, fileName);

        //        using (var fileStream = new FileStream(outputFile, FileMode.Create))
        //        {
        //            await file.CopyToAsync(fileStream);
        //        }

        //        if (file.Name == nameof(model.CancelledCheque))
        //        {
        //            fileUpload.CancelledCheque = file.Name;
        //            fileUpload.IsCancelledCheque = true;
        //        }
        //        else if (file.Name == nameof(model.PaySlipPdf))
        //        {
        //            fileUpload.PaySlipPdf = file.Name;
        //            fileUpload.IsPaySlipPdf = true;
        //        }
        //        else if (file.Name == nameof(model.QuotationPdf))
        //        {
        //            fileUpload.QuotationPdf = file.Name;
        //            fileUpload.IsQuotationPdf = true;
        //        }
        //        else if (file.Name == nameof(model.DrivingLicensePdf))
        //        {
        //            fileUpload.DrivingLicensePdf = file.Name;
        //            fileUpload.IsDrivingLicensePdf = true;
        //        }
        //        else if (file.Name == nameof(model.SeviceExtnPdf))
        //        {
        //            fileUpload.SeviceExtnPdf = file.Name;
        //            fileUpload.IsSeviceExtnPdf = true;
        //        }
        //    }

        //    await _IDocumentUpload.Add(fileUpload);

        //    await _IonlineApplication1.UpdateApplicationStatus(applicationId, 1);
        //    TrnStatusCounter trnStatusCounter = new TrnStatusCounter
        //    {
        //        StatusId = 1,
        //        ApplicationId = applicationId,
        //        ActionOn = DateTime.Now,
        //    };
        //    await _IonlineApplication1.InsertStatusCounter(trnStatusCounter);


        //    var IOArmyNo = await _IonlineApplication1.GetIOArmyNoAsync(applicationId);
        //    if (IOArmyNo == null)
        //    {
        //            var CoDetails = await _IonlineApplication1.GetCoDetails(applicationId);
        //            if (CoDetails != null)
        //            {
        //                TrnFwdCO trnFwdCO = new TrnFwdCO
        //                {
        //                    ApplicationId = applicationId,
        //                    ArmyNo = ArmyNo,
        //                    COUserId = CoDetails.UserId,
        //                    ProfileId = CoDetails.ProfileId,
        //                    CreatedOn = DateTime.Now,
        //                    Status = 1
        //                };
        //                await _IonlineApplication1.AddFwdCO(trnFwdCO);
        //            }

        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(IOArmyNo))
        //        {
        //            var IoDetails = await _IonlineApplication1.GetUserDetails(IOArmyNo);
        //            if (IoDetails != null)
        //            {
        //                TrnFwdCO trnFwdCO = new TrnFwdCO
        //                {
        //                    ApplicationId = applicationId,
        //                    ArmyNo = ArmyNo,
        //                    COUserId = IoDetails.UserId,
        //                    ProfileId = IoDetails.ProfileId,
        //                    CreatedOn = DateTime.Now,
        //                    Status = 1
        //                };
        //                await _IonlineApplication1.AddFwdCO(trnFwdCO);
        //            }

        //        }
        //    }

        //    TempData["Message"] = "Application has been forwarded to your Unit Cdr/IO/Superior Countersigning Auth.";
        //    return RedirectToAction("ApplicationDetails","Upload");
        //}

        public async Task<IActionResult> Upload(FileUploadViewModel model, string formType, int applicationId)
        {
            TempData.Keep("applicationId");

            var ArmyNo = await GetArmyNumber(applicationId);
            if (string.IsNullOrEmpty(ArmyNo)) ArmyNo = string.Empty;

            formType = await GetFormType(formType, applicationId);

            var files = GetUploadedFiles(model);
            if (!files.Any())
            {
                ModelState.AddModelError("", "Please upload at least one file.");
                return View("Upload", model);
            }

            if (!await ValidateFiles(files))
            {
                return View("Upload", model);
            }

            string folderPath = PrepareFolder(formType, ArmyNo, applicationId);

            var fileUpload = await SaveFiles(model, files, folderPath, applicationId);

            await _IDocumentUpload.Add(fileUpload);

            await _IonlineApplication1.UpdateApplicationStatus(applicationId, 1);

            var status = new TrnStatusCounter
            {
                StatusId = 1,
                ApplicationId = applicationId,
                ActionOn = DateTime.Now
            };

            await _IonlineApplication1.InsertStatusCounter(status);

            await ForwardApplication(applicationId, ArmyNo);

            TempData["Message"] = "Application has been forwarded to your Unit Cdr/IO/Superior Countersigning Auth.";
            return RedirectToAction("ApplicationDetails", "Upload");
        }

        // ------------------- Helpers -------------------

        private async Task<string> GetArmyNumber(int applicationId)
        {
            if (applicationId == 0) return string.Empty;

            var commonDataModel = await _IonlineApplication1.Get(applicationId);
            if (commonDataModel == null) return string.Empty;

            var mArmyPrefix = await _IArmyPrefixes.Get(commonDataModel.ArmyPrefix);
            return $"{mArmyPrefix?.Prefix ?? ""}{commonDataModel.Number ?? ""}{commonDataModel.Suffix ?? ""}".Trim();
        }

        private async Task<string> GetFormType(string formType, int applicationId)
        {
            if (formType != "CA") return formType;

            int vehType = await _IonlineApplication1.GetVehicleType(applicationId, formType);
            return vehType == 4 ? "TW" : "CAR";
        }

        private List<IFormFile> GetUploadedFiles(FileUploadViewModel model)
        {
            var files = new List<IFormFile>();
            if (model.CancelledCheque != null) files.Add(model.CancelledCheque);
            if (model.PaySlipPdf != null) files.Add(model.PaySlipPdf);
            if (model.QuotationPdf != null) files.Add(model.QuotationPdf);
            if (model.DrivingLicensePdf != null) files.Add(model.DrivingLicensePdf);
            if (model.SeviceExtnPdf != null) files.Add(model.SeviceExtnPdf);
            return files;
        }

        private async Task<bool> ValidateFiles(List<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.ContentType != "application/pdf")
                    ModelState.AddModelError(file.Name, "Only PDF files are allowed.");

                if (file.Length > 150 * 1024)                
                    ModelState.AddModelError(file.Name, "File size must not exceed 150 KB.");
                
                if (file.Length > 1 * 1024 * 1024)
                    ModelState.AddModelError(file.Name, "File size must not exceed 1 MB.");

                if (!await _pdfUpload.IsValidPdfFile(file))
                    ModelState.AddModelError(file.Name, "File is not a valid PDF or appears to be a disguised file type.");

                if (await _pdfUpload.IsPdfPasswordProtected(file))
                    ModelState.AddModelError(file.Name, "Password-protected PDF are not allowed.");

                if (await _pdfUpload.ContainsMaliciousPdfContent(file))
                    ModelState.AddModelError(file.Name, "PDF contains potentially malicious content.");
            }

            return ModelState.IsValid;
        }

        private string PrepareFolder(string formType, string ArmyNo, int applicationId)
        {
            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempUploads");
            Directory.CreateDirectory(tempFolder);

            string folderName = $"{formType}{ArmyNo}_{applicationId}";
            string folderPath = Path.Combine(tempFolder, folderName);
            Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        private async Task<DocumentUpload> SaveFiles(FileUploadViewModel model, List<IFormFile> files, string folderPath, int applicationId)
        {
            var fileUpload = new DocumentUpload { ApplicationId = applicationId };

            var fileMap = new Dictionary<string, Action>
               {
                   { nameof(model.CancelledCheque), () => { fileUpload.CancelledCheque = nameof(model.CancelledCheque); fileUpload.IsCancelledCheque = true; } },
                   { nameof(model.PaySlipPdf), () => { fileUpload.PaySlipPdf = nameof(model.PaySlipPdf); fileUpload.IsPaySlipPdf = true; } },
                   { nameof(model.QuotationPdf), () => { fileUpload.QuotationPdf = nameof(model.QuotationPdf); fileUpload.IsQuotationPdf = true; } },
                   { nameof(model.DrivingLicensePdf), () => { fileUpload.DrivingLicensePdf = nameof(model.DrivingLicensePdf); fileUpload.IsDrivingLicensePdf = true; } },
                   { nameof(model.SeviceExtnPdf), () => { fileUpload.SeviceExtnPdf = nameof(model.SeviceExtnPdf); fileUpload.IsSeviceExtnPdf = true; } },
               };

            foreach (var file in files)
            {
                string outputFile = Path.Combine(folderPath, $"{file.Name}{Path.GetExtension(file.FileName)}");
                using (var fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                if (fileMap.ContainsKey(file.Name))
                {
                    fileMap[file.Name].Invoke();
                }
            }

            return fileUpload;
        }

        private async Task ForwardApplication(int applicationId, string ArmyNo)
        {
            var IOArmyNo = await _IonlineApplication1.GetIOArmyNoAsync(applicationId);
            if (string.IsNullOrEmpty(IOArmyNo))
            {
                var coDetails = await _IonlineApplication1.GetCoDetails(applicationId);
                if (coDetails != null)
                {
                    await AddFwdCO(applicationId, ArmyNo, coDetails.UserId, coDetails.ProfileId);
                }
            }
            else
            {
                var ioDetails = await _IonlineApplication1.GetUserDetails(IOArmyNo);
                if (ioDetails != null)
                {
                    await AddFwdCO(applicationId, ArmyNo, ioDetails.UserId, ioDetails.ProfileId);
                }
            }
        }

        private async Task AddFwdCO(int applicationId, string ArmyNo, int userId, int profileId)
        {
            var trnFwdCO = new TrnFwdCO
            {
                ApplicationId = applicationId,
                ArmyNo = ArmyNo,
                COUserId = userId,
                ProfileId = profileId,
                CreatedOn = DateTime.Now,
                Status = 1
            };

            await _IonlineApplication1.AddFwdCO(trnFwdCO);
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
