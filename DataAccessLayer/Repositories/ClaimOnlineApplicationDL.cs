using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ClaimOnlineApplicationDL : GenericRepositoryDL<ClaimCommonModel>, IClaimOnlineApplication
    {

        protected new readonly ApplicationDbContext _context;

        public ClaimOnlineApplicationDL(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }

        public bool ValidateFileUpload(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Check if file is null
            if (file == null)
            {
                errorMessage = "No file selected.";
                return false;
            }

            // Check if the file type is PDF
            if (file.ContentType != "application/pdf")
            {
                errorMessage = "Only PDF files are allowed.";
                return false;
            }

            // Check if the file size exceeds 1 MB (1 * 1024 * 1024 bytes)
            if (file.Length > 1 * 1024 * 1024)
            {
                errorMessage = "File size cannot exceed 1 MB.";
                return false;
            }

            return true;
        }
    }
}
