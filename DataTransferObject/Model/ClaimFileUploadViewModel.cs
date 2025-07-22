using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class ClaimFileUploadViewModel
    {
        public IFormFile? CancelledCheque { get; set; }

        public IFormFile? PaySlipPdf { get; set; }

        public IFormFile? SpdocusPdf { get; set; }

        public IFormFile? SeviceExtnPdf { get; set; }

        public string? FormType { get; set; }
    }
}
