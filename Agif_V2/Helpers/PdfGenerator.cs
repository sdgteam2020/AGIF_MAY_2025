using DataAccessLayer.Interfaces;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Path = System.IO.Path;

namespace Agif_V2.Helpers
{
    public class PdfGenerator
    {
        private readonly IOnlineApplication _usersApplications;
        public PdfGenerator(IOnlineApplication usersApplications)
        {
            _usersApplications = usersApplications;
        }
        public async Task<int> CreatePdfForOnlineApplication(int applicationId, string generatedPdfPath)
        {
            var data = await _usersApplications.GetApplicationDetails(applicationId);
            var directory = Path.GetDirectoryName(generatedPdfPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var writer = new PdfWriter(generatedPdfPath))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    var pageSize = PageSize.A4; // Use PageSize directly
                    var document = new Document(pdf, pageSize); // Use PageSize instead of Rectangle
                    document.SetMargins(30f, 30f, 30f, 30f);

                    //Define Fonts
                    PdfFont titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    PdfFont subtitleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    // Create a 2-column table
                    Table headerTable = new Table(2).UseAllAvailableWidth();

                    // Set column widths (optional)
                    headerTable.SetWidth(UnitValue.CreatePercentValue(100));
                    headerTable.SetWidth(UnitValue.CreatePercentValue(100));

                    // Left: Date & Time
                    Cell dateCell = new Cell()
                        .Add(new Paragraph(DateTime.Now.ToString("M/d/yy, h:mm tt")).SetFont(normalFont))
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.LEFT);
                    headerTable.AddCell(dateCell);

                    // Right: Army No + Filename
                    string armyNoFile = data.OnlineApplicationResponse.Number+"_AGIF_Loan_ApplForm";
                    Cell armyNoCell = new Cell()
                        .Add(new Paragraph(armyNoFile).SetFont(normalFont))
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.RIGHT);
                    headerTable.AddCell(armyNoCell);

                    // Add to document
                    document.Add(headerTable);

                    // Add sample content
                    document.Add(new Paragraph($"Application ID: {applicationId}"));
                    document.Add(new Paragraph($"Applicant Name: {data?.OnlineApplicationResponse?.ApplicantName ?? "Unknown"}")); // Use null-conditional operator
                    document.Add(new Paragraph("This is a generated PDF using iText7."));

                    // Close document
                    document.Close();
                }
            }
            return 1;
        }
    }
}
