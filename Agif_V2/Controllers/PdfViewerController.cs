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
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace Agif_V2.Controllers
{
    public class PdfViewerController : Controller
    {
        private readonly IWebHostEnvironment _env;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        public PdfViewerController(IWebHostEnvironment env)
        {
            _env = env;
            //_httpContextAccessor = httpContextAccessor;
        }
        public IActionResult AGIJournal()
        {
            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "AGI Journal.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult AGIFLoanInwardPaymentUsingNEFT()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "AGIF Loan  Inward Payment Using NEFT.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult BenevolentReserveFund()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "Benevolent Reserve Fund.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult Fraud()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "FRAUD.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult PreEmiRevision()
        {
            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "PRE-EMI REVISION.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult ProvisioningofITRebateCertificate()
        {
            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "Provisioning of IT Rebate Certificate .pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult ReviewOfHBA()
        {
            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "Review of HBA.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult RevisedNominationForm()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "REVISED NOMINATION FORM.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult RevisionOfPolicyForGrantOfHba()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "REVISION OF POLICY FOR GRANT OF HBA.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult FaqClaims()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "FAQs", "faq_claims.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult FaqAdvancesGeneral()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "FAQs", "faq_advances_general.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult FaqHBA()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "FAQs", "faq_HBA.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult FaqCA()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "FAQs", "faq_CA.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult FaqPCA()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "FAQs", "faq_PCA.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult SavingElement()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "Saving Element.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult AgifInfoBrochure()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "Info Brochure.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult GuidelinesUnitCdr()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "GuidelinesOfUnitCdr.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult UserManual()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "UserManual.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult ClaimInstruction()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "OnlineApplnProcedureforCA_Dec2024.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult MAWD()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "MAWD.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult HbaApplication()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "HBA Application.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult InstrHBA()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "OnlineApplnProcedureforHBA.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult InstrCA()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "OnlineApplnProcedureforCA_Dec2024.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        public IActionResult InstrPCA()
        {

            string inputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "OnlineApplnProcedureforPCA_Dec2024.pdf");
            ReaderProperties readerProperties = new ReaderProperties();
            PdfReader pdfReader = new PdfReader(inputPath, readerProperties);
            OpenPdf(pdfReader);
            string outputPath = System.IO.Path.Combine(_env.WebRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            if (!System.IO.File.Exists(outputPath))
            {
                return NotFound("The PDF file could not be generated.");
            }

            var stream = new FileStream(outputPath, FileMode.Open, FileAccess.Read);
            Response.Headers.Append("Content-Disposition", "inline; filename=TempPdf.pdf");
            return File(stream, "application/pdf");

        }
        
        public void OpenPdf(PdfReader pdfReader)
        {
            string ipAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
            string wwwRootPath = _env.WebRootPath;
            string outputPath = System.IO.Path.Combine(wwwRootPath, "ImportantPdfFiles", "TempPdf.pdf");
            using (var memoryStream = new MemoryStream())
            {
                var writerProperties = new WriterProperties().UseSmartMode();
                using (var writer = new PdfWriter(memoryStream, writerProperties))
                using (var pdfDoc = new PdfDocument(pdfReader, writer))
                {
                    for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                    {
                        var page = pdfDoc.GetPage(i);
                        var pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
                        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                        var canvas = new Canvas(pdfCanvas, page.GetPageSize());
                        string ipDisplay = string.IsNullOrEmpty(ipAddress) ? "IP Address Not Found!" : ipAddress.Replace(" ", "\u00A0");
                        string dateTimeDisplay = DateTime.Now.ToLocalTime().ToString("dd/MM/yyyy HH:mm").Trim().Replace(" ", "\u00A0");
                        Color watermarkColor = new DeviceRgb(150, 150, 150);
                        pdfCanvas.SaveState();
                        PdfExtGState gState = new PdfExtGState().SetFillOpacity(0.5f);
                        pdfCanvas.SetExtGState(gState);
                        var ipParagraph = new Paragraph(ipDisplay)
                            .SetFont(font)
                            .SetFontSize(50)
                            .SetRotationAngle(Math.PI / 4)
                            .SetFontColor(watermarkColor);

                        var dateParagraph = new Paragraph(dateTimeDisplay)
                            .SetFont(font)
                            .SetFontSize(50)
                            .SetRotationAngle(Math.PI / 4)
                            .SetFontColor(watermarkColor);
                        canvas.ShowTextAligned(ipParagraph, 300, 420, i, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 0);
                        canvas.ShowTextAligned(dateParagraph, 300, 350, i, TextAlignment.CENTER, VerticalAlignment.MIDDLE, 0);
                    }
                }
                System.IO.File.WriteAllBytes(outputPath, memoryStream.ToArray());
            }
            pdfReader.Close();
        }

    }
}
