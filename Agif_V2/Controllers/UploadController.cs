using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.StyledXmlParser.Node;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> ApplicationDetails(int applicationId,string formtype)
        {
            if (applicationId == 0)
            {
                return NotFound();
            }

            var application = await _IonlineApplication1.GetApplicationDetails(applicationId, formtype);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // Update ProcessUpload to redirect to ApplicationDetails after upload
        [HttpPost]
        public async Task<IActionResult> ProcessUpload(FileUploadViewModel model, string formType, int applicationId)
        {
            CommonDataModel commonDataModel = new CommonDataModel();
            MArmyPrefix mArmyPrefix = new MArmyPrefix();
            string ArmyNo = string.Empty;

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


            TempData["Message"] = "Document Uploaded Successfully and forwarded to Unit Commander.";
            return RedirectToAction("ApplicationDetails", new { applicationId = applicationId,formType = formType });
            //TempData["Message"] = "Document Uploaded Successfully and forwarded to Unit Commander.";
            //return View("Upload", model);
        }
        public IActionResult UploadSuccess()
        {
            return View();
        }
    }
}
