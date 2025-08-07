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
using iText.Kernel.Pdf.Annot;
using Microsoft.AspNetCore.Components.Forms;

namespace Agif_V2.Helpers
{
    public class Watermark
    {
        public void OpenPdf(PdfReader pdfReader, string ipAddress, string wwwRootPath)
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
        //public void OpenPdf1(string ipAddress, string wwwRootPath)
        //{
        //    string tempOutputPath = Path.Combine(Path.GetDirectoryName(wwwRootPath), "temp_" + Path.GetFileName(wwwRootPath));

        //    using (var reader = new PdfReader(wwwRootPath))
        //    using (var writer = new PdfWriter(tempOutputPath))
        //    using (var pdfDoc = new PdfDocument(reader, writer))
        //    {
        //        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        //        string ipDisplay = string.IsNullOrEmpty(ipAddress) ? "IP Address Not Found!" : ipAddress.Replace(" ", "\u00A0");
        //        string dateTimeDisplay = DateTime.Now.ToString("dd/MM/yyyy HH:mm").Replace(" ", "\u00A0");

        //        for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
        //        {
        //            PdfPage page = pdfDoc.GetPage(i);
        //            var annotation = new PdfFreeTextAnnotation(
        //                new iText.Kernel.Geom.Rectangle(100, 100, 400, 100),
        //                new PdfString($"{ipDisplay}\n{dateTimeDisplay}"))
        //                .SetDefaultAppearance(new PdfString("/Helv 12 Tf 0.5 g"))
        //                .SetColor(ColorConstants.LIGHT_GRAY)
        //                .SetFlags(PdfAnnotation.PRINT);

        //            page.AddAnnotation(annotation);
        //        }
        //    }
        //    // Now the file is released, safe to overwrite
        //    File.Copy(tempOutputPath, wwwRootPath, overwrite: true);
        //    File.Delete(tempOutputPath);
        //}


    }
}
