using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class DocumentUpload:Common
    {
        [Key]
        public int UploadId { get; set; }

        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public CommonDataModel? CommonDataModels { get; set; }

        public string? CancelledCheque { get; set; }
        public string? PaySlipPdf { get; set; }
        public string? QuotationPdf { get; set; }
        public string? DrivingLicensePdf { get; set; }
        public string? SeviceExtnPdf { get; set; }


        public bool IsCancelledCheque { get; set; }

        public bool IsPaySlipPdf { get; set; }

        public bool IsQuotationPdf { get;set; }

        public bool IsDrivingLicensePdf { get; set; }

        public bool IsSeviceExtnPdf { get; set; }
    }
}
