using iText.IO.Font.Constants;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Agif_V2.Helpers
{
    public class Watermark
    {
        public void OpenPdf(PdfReader pdfReader,string ipAddress,string wwwRootPath)
        {
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

                        pdfCanvas.RestoreState();
                    }
                }
                System.IO.File.WriteAllBytes(wwwRootPath, memoryStream.ToArray());
            }
            pdfReader.Close();
        }
    }
}
