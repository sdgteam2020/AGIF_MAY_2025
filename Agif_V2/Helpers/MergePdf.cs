using iText.Kernel.Pdf;
using iText.Kernel.Utils;

namespace Agif_V2.Helpers
{
    public class MergePdf
    {
        public async Task<bool> MergePdfFiles(string[] inputFiles, string outputPath)
        {
            try
            {
                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                foreach (string file in inputFiles)
                {
                    if (!File.Exists(file))
                    {
                        Console.WriteLine($"Input file not found: {file}");
                        return false;
                    }
                }

                using (var outputStream = new FileStream(outputPath, FileMode.Create))
                {
                    using (var pdfWriter = new PdfWriter(outputStream))
                    {
                        using (var pdfDocument = new PdfDocument(pdfWriter))
                        {
                            var merger = new PdfMerger(pdfDocument);

                            foreach (string file in inputFiles)
                            {
                                try
                                {
                                    using (var inputStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                                    {
                                        using (var inputPdfReader = new PdfReader(inputStream))
                                        {
                                            using (var inputPdfDocument = new PdfDocument(inputPdfReader))
                                            {
                                                merger.Merge(inputPdfDocument, 1, inputPdfDocument.GetNumberOfPages());
                                            }
                                        }
                                    }
                                }
                                catch (Exception fileEx)
                                {
                                    Console.WriteLine($"Error processing file {file}: {fileEx.Message}");
                                    continue;
                                }
                            }
                        }
                    }
                }

                if (File.Exists(outputPath))
                {
                    Console.WriteLine($"PDF merged successfully: {outputPath}");
                    return true;
                }
                else
                {
                    Console.WriteLine("Output file was not created");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}