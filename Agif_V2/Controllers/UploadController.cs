using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Agif_V2.Controllers
{
    public class UploadController : Controller
    {
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessUpload(IFormFile CancelledCheque,IFormFile PaySlipPdf,IFormFile QuotationPdf,IFormFile DrivingLicensePdf,IFormFile SeviceExtnPdf,string formType)
        {
            var files = new List<IFormFile>();
            if (CancelledCheque != null) files.Add(CancelledCheque);
            if (PaySlipPdf != null) files.Add(PaySlipPdf);
            if (QuotationPdf != null) files.Add(QuotationPdf);
            if (DrivingLicensePdf != null) files.Add(DrivingLicensePdf);
            if (SeviceExtnPdf != null) files.Add(SeviceExtnPdf);

            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempUploads");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
            string outputFile = Path.Combine(tempFolder, $"{formType}_Merged_{DateTime.Now:ddMMyyyy_HHmmss}.pdf");
            using (var outputStream = new FileStream(outputFile, FileMode.Create))
            {
                using (var pdfWriter = new PdfWriter(outputStream))
                {
                    using (var pdfDoc = new PdfDocument(pdfWriter))
                    {
                        var merger = new PdfMerger(pdfDoc);

                        foreach (var file in files)
                        {
                            using (var ms = new MemoryStream())
                            {
                                await file.CopyToAsync(ms);
                                ms.Position = 0;

                                var reader = new PdfReader(ms);
                                using (var srcDoc = new PdfDocument(reader))
                                {
                                    merger.Merge(srcDoc, 1, srcDoc.GetNumberOfPages());
                                }
                            }
                        }
                    }
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
