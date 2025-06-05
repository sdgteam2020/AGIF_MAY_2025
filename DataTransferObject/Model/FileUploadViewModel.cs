using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class FileUploadViewModel
    {
      
        public IFormFile? CancelledCheque { get; set; }


        public IFormFile? PaySlipPdf { get; set; }

      
        public IFormFile? QuotationPdf { get; set; }

      
        public IFormFile? DrivingLicensePdf { get; set; }

   
        public IFormFile? SeviceExtnPdf { get; set; }
    }
}
