using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Agif_V2.Helpers
{
    public class PdfUpload
    {
        //public async Task<bool> IsValidPdfFile(IFormFile file)
        //{
        //    try
        //    {
        //        using var stream = file.OpenReadStream();

        //        var buffer = new byte[8];
        //        await stream.ReadAsync(buffer, 0, 8);

        //        var pdfSignature = System.Text.Encoding.ASCII.GetString(buffer, 0, 5);
        //        if (pdfSignature != "%PDF-")
        //        {
        //            return false;
        //        }

        //        stream.Seek(-1024, SeekOrigin.End); // Check last 1KB
        //        var endBuffer = new byte[1024];
        //        await stream.ReadAsync(endBuffer, 0, 1024);
        //        var endContent = System.Text.Encoding.ASCII.GetString(endBuffer);

        //        if (!endContent.Contains("%%EOF"))
        //        {
        //            return false;
        //        }

        //        stream.Seek(0, SeekOrigin.Begin);
        //        var fullBuffer = new byte[512];
        //        await stream.ReadAsync(fullBuffer, 0, 512);

        //        if (fullBuffer.Length >= 4 &&
        //            fullBuffer[0] == 0x50 && fullBuffer[1] == 0x4B &&
        //            fullBuffer[2] == 0x03 && fullBuffer[3] == 0x04)
        //        {
        //            return false; // This is likely a ZIP file (Word document)
        //        }

        //        if (fullBuffer.Length >= 8 &&
        //            fullBuffer[0] == 0xD0 && fullBuffer[1] == 0xCF &&
        //            fullBuffer[2] == 0x11 && fullBuffer[3] == 0xE0 &&
        //            fullBuffer[4] == 0xA1 && fullBuffer[5] == 0xB1 &&
        //            fullBuffer[6] == 0x1A && fullBuffer[7] == 0xE1)
        //        {
        //            return false; // This is an OLE2 file (old Word document)
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        public async Task<bool> IsValidPdfFile(IFormFile file)
        {
            try
            {

                using var stream = file.OpenReadStream();

                byte[] header = new byte[8];
                int bytesRead = await stream.ReadAsync(header, 0, header.Length);

                if (bytesRead < 5)
                    return false;

                string signature = Encoding.ASCII.GetString(header, 0, 5);

                if (signature != "%PDF-")
                    return false;

                long bytesToRead = Math.Min(1024, stream.Length);

                stream.Seek(-bytesToRead, SeekOrigin.End);

                byte[] footer = new byte[bytesToRead];
                await stream.ReadAsync(footer, 0, footer.Length);

                string footerText = Encoding.ASCII.GetString(footer);

                if (!footerText.Contains("%%EOF"))
                    return false;

                // Optional: Reject encrypted PDFs
                stream.Seek(0, SeekOrigin.Begin);

                byte[] scanBuffer = new byte[Math.Min((int)stream.Length, 1024 * 1024)];
                await stream.ReadAsync(scanBuffer, 0, scanBuffer.Length);

                string content = Encoding.ASCII.GetString(scanBuffer);

                if (content.Contains("/Encrypt"))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> CheckIfPdfPasswordProtected(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();

                var reader = new PdfReader(stream);
                var pdf = new PdfDocument(reader);

                return false;
            }
            catch (BadPasswordException)
            {
                return true;
            }
            catch
            {
                return true;
            }
        }
        public async Task<bool> IsPdfPasswordProtected(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();

                var encryptionPatterns = new[]
                {
                @"/Encrypt\s+\d+\s+\d+\s+R",
                @"/Filter\s*/Standard",
                @"/V\s+[1-5]",
                @"/R\s+[2-6]",
                @"/P\s+-?\d+",
                @"/O\s*<[0-9A-Fa-f]+>",
                @"/U\s*<[0-9A-Fa-f]+>"
                };

                return encryptionPatterns.Any(pattern =>
                    Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
            }
            catch
            {
                return true; // If we can't read it, assume it's protected
            }
        }

        public async Task<bool> ContainsMaliciousPdfContent(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                var content = await reader.ReadToEndAsync();

                var maliciousPatterns = new[]
               {
                @"/JavaScript\s*<<",
                @"/JS\s*\(",
                @"\.eval\s*\(",
                @"\.write\s*\(",
                
                @"/Launch\s*<<",
                @"/Win\s*\(",
                @"/Mac\s*\(",
                @"/Unix\s*\(",
                
                @"/OpenAction\s*<<",
                @"/AA\s*<<",
                
                @"/EmbeddedFile\s*<<",
                @"/EF\s*<<",
                
                @"/SubmitForm\s*<<",
                @"/F\s+4",
                
                @"/RichMedia\s*<<",
                @"/3D\s*<<",
                @"/U3D\s*<<",
                
                @"unescape\s*\(",
                @"String\.fromCharCode\s*\(",
                @"document\.write\s*\(",
                @"eval\s*\(",
                
                @"%u0065%u0076%u0061%u006c", // 'eval' encoded
                @"\\x65\\x76\\x61\\x6c",    // 'eval' hex encoded
              };

                

                return maliciousPatterns.Any(pattern =>
                Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase));
            }
            catch
            {
                return true; // If we can't analyze it, consider it suspicious
            }
        }

        public async Task<bool> ValidateSavedPdfFile(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);

                if (fileInfo.Length == 0)
                    return false;

                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                var headerBuffer = new byte[8];
                await fileStream.ReadAsync(headerBuffer, 0, 8);
                var header = System.Text.Encoding.ASCII.GetString(headerBuffer);

                if (!header.StartsWith("%PDF-"))
                    return false;

                var version = header.Substring(5, 3);
                var validVersions = new[] { "1.0", "1.1", "1.2", "1.3", "1.4", "1.5", "1.6", "1.7", "2.0" };

                if (!validVersions.Contains(version))
                    return false;

                fileStream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(fileStream);
                var content = await reader.ReadToEndAsync();

                if (!content.Contains("obj") || !content.Contains("endobj"))
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateSecureFileName(string extension)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[16];
            rng.GetBytes(bytes);
            return Convert.ToHexString(bytes).ToLower() + extension;
        }

        public async Task<bool> ContainsSuspiciousContent(IFormFile file)
        {
            return await ContainsMaliciousPdfContent(file);
        }

    }
}
