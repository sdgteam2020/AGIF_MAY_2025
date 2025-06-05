using DataTransferObject.Model;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Agif_V2.Controllers
{
    public class UploadController : Controller
    {
        public IActionResult Upload()
        {
            FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
            return View(fileUploadViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessUpload(FileUploadViewModel model, string formType)
        {
            var files = new List<IFormFile>();
            if (model.CancelledCheque != null) files.Add(model.CancelledCheque);
            if (model.PaySlipPdf != null) files.Add(model.PaySlipPdf);
            if (model.QuotationPdf != null) files.Add(model.QuotationPdf);
            if (model.DrivingLicensePdf != null) files.Add(model.DrivingLicensePdf);
            if (model.SeviceExtnPdf != null) files.Add(model.SeviceExtnPdf);
            FileUploadViewModel fileUploadViewModel = new FileUploadViewModel();
            if (files.Count == 0)
            {
                ModelState.AddModelError("", "Please upload at least one file.");
                return View(); 
            }

            // Validate each file (size, type)
            foreach (var file in files)
            {
                // Validate file type (PDF)
                if (file.ContentType != "application/pdf")
                {
                    ModelState.AddModelError(file.Name, "Only PDF files are allowed.");
                }

                // Validate file size (max 1MB)
                if (file.Length > 1 * 1024 * 1024) // 1MB
                {
                    ModelState.AddModelError(file.Name, "File size must not exceed 1 MB.");
                }
            }
            ViewData["formType"] = formType;
            // If there are validation errors, return the view with error messages
            if (!ModelState.IsValid)
            {
                return View("Upload", model); // Show the form again with error messages
            }

            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempUploads");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
           
            string outputFile = string.Empty;
            foreach (var file in files)
            {
                string folderName = file.Name switch
                {
                    "CancelledCheque" => "CancelledCheque",
                    "PaySlipPdf" => "PaySlipPdf",
                    "QuotationPdf" => "QuotationPdf",
                    "DrivingLicensePdf" => "DrivingLicensePdf",
                    "SeviceExtnPdf" => "SeviceExtnPdf",
                    _ => "Others"
                };

                // Create a specific folder for each file type
                string folderPath = Path.Combine(tempFolder, folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Create the output file path
                outputFile = Path.Combine(folderPath, $"{formType}_{Guid.NewGuid()}_{file.FileName}");

                // Save the file to the respective folder
                using (var fileStream = new FileStream(outputFile, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            TempData["Message"] = $"Merged PDF saved to: {outputFile}";
            return RedirectToAction("UploadSuccess");
        }
        public IActionResult UploadSuccess()
        {
            return View();
        }
    }
}
