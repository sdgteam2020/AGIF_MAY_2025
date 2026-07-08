using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace Agif_V2.Controllers
{
    public class PdfViewerController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public PdfViewerController(IWebHostEnvironment env)
        {
            _env = env;
        }


        public IActionResult AGIJournal() => ServeWatermarkedPdf("ImportantPdfFiles", "AGI Journal.pdf");
        public IActionResult AGIFLoanInwardPaymentUsingNEFT() => ServeWatermarkedPdf("ImportantPdfFiles", "AGIF Loan  Inward Payment Using NEFT.pdf");
        public IActionResult BenevolentReserveFund() => ServeWatermarkedPdf("ImportantPdfFiles", "Benevolent Reserve Fund.pdf");
        public IActionResult Fraud() => ServeWatermarkedPdf("ImportantPdfFiles", "FRAUD.pdf");
        public IActionResult PreEmiRevision() => ServeWatermarkedPdf("ImportantPdfFiles", "PRE-EMI REVISION.pdf");
        public IActionResult ProvisioningofITRebateCertificate() => ServeWatermarkedPdf("ImportantPdfFiles", "Provisioning of IT Rebate Certificate .pdf");
        public IActionResult ReviewOfHBA() => ServeWatermarkedPdf("ImportantPdfFiles", "Review of HBA.pdf");
        public IActionResult RevisedNominationForm() => ServeWatermarkedPdf("ImportantPdfFiles", "REVISED NOMINATION FORM.pdf");
        public IActionResult RevisionOfPolicyForGrantOfHba() => ServeWatermarkedPdf("ImportantPdfFiles", "REVISION OF POLICY FOR GRANT OF HBA.pdf");
        public IActionResult SavingElement() => ServeWatermarkedPdf("ImportantPdfFiles", "Saving Element.pdf");
        public IActionResult AgifInfoBrochure() => ServeWatermarkedPdf("ImportantPdfFiles", "Info Brochure.pdf");
        public IActionResult GuidelinesUnitCdr() => ServeWatermarkedPdf("ImportantPdfFiles", "GuidelinesOfUnitCdr.pdf");
        public IActionResult MAWD() => ServeWatermarkedPdf("ImportantPdfFiles", "MAWD.pdf");
        public IActionResult HbaApplication() => ServeWatermarkedPdf("ImportantPdfFiles", "HBA Application.pdf");

        public IActionResult FaqClaims() => ServeWatermarkedPdf("FAQs", "faq_claims.pdf");
        public IActionResult FaqAdvancesGeneral() => ServeWatermarkedPdf("FAQs", "faq_advances_general.pdf");
        public IActionResult FaqHBA() => ServeWatermarkedPdf("FAQs", "faq_HBA.pdf");
        public IActionResult FaqCA() => ServeWatermarkedPdf("FAQs", "faq_CA.pdf");
        public IActionResult FaqPCA() => ServeWatermarkedPdf("FAQs", "faq_PCA.pdf");
        public IActionResult InstrARPAN() => ServeWatermarkedPdf("FAQs", "faq_PCA.pdf");

        public IActionResult ClaimInstruction() => ServeWatermarkedPdf("ImportantPdfFiles", "OnlineApplnProcedureforCA_Dec2024.pdf");
        public IActionResult InstrHBA() => ServeWatermarkedPdf("ImportantPdfFiles", "OnlineApplnProcedureforHBA.pdf");
        public IActionResult InstrCA() => ServeWatermarkedPdf("ImportantPdfFiles", "OnlineApplnProcedureforCA_Dec2024.pdf");
        public IActionResult InstrPCA() => ServeWatermarkedPdf("ImportantPdfFiles", "OnlineApplnProcedureforPCA_Dec2024.pdf");
        public IActionResult UserManual() => ServeWatermarkedPdf("ImportantPdfFiles", "UserManual.pdf");

        //public IActionResult UserManual(bool applyWatermark = false)
        //{
        //    if (!applyWatermark)
        //    {
        //        string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "UserManual.pdf");
        //        if (!System.IO.File.Exists(inputPath)) return NotFound("Document not found.");

        //        Response.Headers.Append("Content-Disposition", "inline; filename=UserManual.pdf");
        //        return PhysicalFile(inputPath, "application/pdf");
        //    }

        //    return ServeWatermarkedPdf("ImportantPdfFiles", "c.pdf");
        //}


        public IActionResult ServeWatermarkedPdf(string folderName, string fileName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request.");
            }
            folderName = folderName.TrimStart('/', '\\');
            string inputPath = System.IO.Path.Combine(_env.WebRootPath, folderName, fileName);
            if (!System.IO.File.Exists(inputPath))
            {
                return NotFound("The requested PDF file could not be found.");
            }

            try
            {
                using var pdfReader = new PdfReader(inputPath, new ReaderProperties());
                byte[] pdfBytes = ApplyWatermark(pdfReader);

                Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while generating the document.");
            }
        }
        private byte[] ApplyWatermark(PdfReader pdfReader)
        {
            string ipAddress = GetClientIp();
            using var memoryStream = new MemoryStream();
            var writerProperties = new WriterProperties().UseSmartMode();

            using (var writer = new PdfWriter(memoryStream, writerProperties))
            using (var pdfDoc = new PdfDocument(pdfReader, writer))
            {
                var font = PdfFontFactory.CreateFont(
                    StandardFonts.HELVETICA,
                    PdfEncodings.WINANSI,
                    PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

                string ipDisplay = string.IsNullOrEmpty(ipAddress)
                    ? "IP Address Not Found!"
                    : ipAddress.Replace(" ", "\u00A0");
                string dateTimeDisplay = DateTime.Now.ToLocalTime()
                    .ToString("dd/MM/yyyy HH:mm").Trim().Replace(" ", "\u00A0");
                Color watermarkColor = new DeviceRgb(150, 150, 150);

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var page = pdfDoc.GetPage(i);
                    int rotation = page.GetRotation();
                    var mediaBox = page.GetMediaBox(); // ✅ Use GetMediaBox() not GetPageSize()

                    // ✅ Always use raw mediaBox width/height for center — iText7 handles
                    // rotation display internally, coordinates are always in unrotated space
                    float centerX = mediaBox.GetLeft() + mediaBox.GetWidth() / 2;
                    float centerY = mediaBox.GetBottom() + mediaBox.GetHeight() / 2;

                    var pdfCanvas = new PdfCanvas(
                        page.NewContentStreamAfter(),
                        page.GetResources(),
                        pdfDoc);

                    var canvas = new Canvas(pdfCanvas, mediaBox);

                    pdfCanvas.SaveState();

                    PdfExtGState gState = new PdfExtGState().SetFillOpacity(0.5f);
                    pdfCanvas.SetExtGState(gState);

                    var watermarkText = new Paragraph(ipDisplay + "\n" + dateTimeDisplay)
                        .SetFont(font)
                        .SetFontSize(40)
                        .SetFontColor(watermarkColor)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMultipliedLeading(1.2f);

                    // ✅ Fixed angle — always diagonal relative to the UNROTATED page
                    // No need to subtract rotation; iText7 content stream is in unrotated space
                    float radAngle = rotation switch
                    {
                        90 => 0f,                          // page displayed landscape → watermark horizontal looks diagonal
                        270 => 0f,
                        180 => -(float)(Math.PI / 4),       // upside-down → mirror flip corrected
                        _ => (float)(Math.PI / 4)         // 0° portrait → standard 45°
                    };

                    canvas.ShowTextAligned(
                        watermarkText,
                        centerX,
                        centerY,
                        i,
                        TextAlignment.CENTER,
                        VerticalAlignment.MIDDLE,
                        radAngle);

                    pdfCanvas.RestoreState();
                    canvas.Close();
                }
            }

            return memoryStream.ToArray();
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
    }
}