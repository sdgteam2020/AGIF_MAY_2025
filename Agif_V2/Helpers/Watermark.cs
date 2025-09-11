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
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Geom;
using Path = System.IO.Path;

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
        
        public void AddAnnotationAfterDigitalSign(string ipAddress, string wwwRootPath)
        {
            string tempOutputPath = wwwRootPath + ".tmp";

            using (var reader = new PdfReader(wwwRootPath))
            using (var writer = new PdfWriter(tempOutputPath, new WriterProperties().SetPdfVersion(PdfVersion.PDF_1_7)))
            using (var pdfDoc = new PdfDocument(reader, writer, new StampingProperties().UseAppendMode()))
            {
                var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                string ipDisplay = string.IsNullOrWhiteSpace(ipAddress) ? "IP Address Not Found!" : ipAddress.Replace(" ", "\u00A0");
                string dateTimeDisplay = DateTime.Now.ToString("dd/MM/yyyy HH:mm").Replace(" ", "\u00A0");

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    PdfPage page = pdfDoc.GetPage(i);
                    Rectangle pageSize = page.GetPageSize();
                    float pw = pageSize.GetWidth();
                    float ph = pageSize.GetHeight();

                    // Create a full-page appearance to mimic a real watermark
                    var appearance = new PdfFormXObject(new Rectangle(0, 0, pw, ph));
                    var appCanvas = new PdfCanvas(appearance, pdfDoc);

                    // Semi-transparent drawing
                    var gs = new PdfExtGState().SetFillOpacity(0.35f); // ~35% opacity
                    appCanvas.SaveState();
                    appCanvas.SetExtGState(gs);

                    // Layout on the appearance using high-level Canvas API
                    using (var layout = new Canvas(appCanvas, new Rectangle(0, 0, pw, ph)))
                    {
                        layout.SetFont(font)
                              .SetFontSize(50)
                              .SetFontColor(new DeviceRgb(150, 150, 150));

                        float cx = pw / 2f;
                        float cy = ph / 2f;
                        float angle = (float)(Math.PI / 4); // 45°

                        var ipParagraph = new Paragraph(ipDisplay)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            .SetMargin(0);

                        var dtParagraph = new Paragraph(dateTimeDisplay)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                            .SetMargin(0);

                        layout.ShowTextAligned(ipParagraph, cx, cy + 35, i,
                                               TextAlignment.CENTER, VerticalAlignment.MIDDLE, angle);

                        layout.ShowTextAligned(dtParagraph, cx, cy - 35, i,
                                               TextAlignment.CENTER, VerticalAlignment.MIDDLE, angle);
                    }

                    appCanvas.RestoreState();

                    // Build an annotation that covers the whole page but has no visible border/background;
                    // its *appearance* is the watermark we drew above.
                    var annotRect = new Rectangle(0, 0, pw, ph);
                    var stamp = new PdfStampAnnotation(annotRect);

                    // Set the appearance we created as the normal appearance
                    stamp.SetNormalAppearance(appearance.GetPdfObject());

                    // Important flags for print and stability
                    stamp.SetFlags(
                        PdfAnnotation.PRINT |
                        PdfAnnotation.READ_ONLY |
                        PdfAnnotation.LOCKED |
                        PdfAnnotation.NO_ZOOM |
                        PdfAnnotation.NO_ROTATE
                    );

                    // No border
                    stamp.SetBorder(new PdfArray(new float[] { 0, 0, 0 }));

                    // Transparent annotation background (the appearance already draws semi-transparent text)
                    //stamp.SetColor(ColorConstants.WHITE, 0); // transparent

                    // Handle page rotation so the watermark looks correct on rotated pages
                    int rotation = page.GetRotation();
                    if (rotation != 0)
                    {
                        // Counter-rotate the appearance so it stays diagonal relative to visual page
                        stamp.Put(PdfName.Rotate, new PdfNumber((360 - rotation) % 360));
                    }

                    page.AddAnnotation(stamp);
                }
            }

            // Replace original file atomically
            if (File.Exists(wwwRootPath)) File.Delete(wwwRootPath);
            File.Move(tempOutputPath, wwwRootPath);
        }

    }
}
