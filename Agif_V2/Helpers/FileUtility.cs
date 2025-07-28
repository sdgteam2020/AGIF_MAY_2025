namespace Agif_V2.Helpers
{
    public class FileUtility
    {
        public async Task SaveBase64ToFileAsync(string base64String, string directoryPath, string fileName)
        {
            try
            {
                // Remove data URI prefix if present
                if (base64String.StartsWith("data:"))
                {
                    base64String = base64String.Substring(base64String.IndexOf(',') + 1);
                }

                // Convert Base64 string to byte array
                byte[] fileBytes = Convert.FromBase64String(base64String);

                // Ensure the directory exists
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Combine directory and file name to form full path
                string fullPath = Path.Combine(directoryPath, fileName);

                // Write file asynchronously
                await File.WriteAllBytesAsync(fullPath, fileBytes);

                Console.WriteLine("File saved successfully at: " + fullPath);
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Invalid Base64 string: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving file: " + ex.Message);
            }
        }
    }
}
