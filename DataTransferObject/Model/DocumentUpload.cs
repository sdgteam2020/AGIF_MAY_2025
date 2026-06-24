using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class DocumentUpload
    {
        [Key]
        public int UploadId { get; set; }

        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public CommonDataModel? CommonDataModels { get; set; }
        public bool IsCancelledCheque { get; set; }

        public bool IsPaySlipPdf { get; set; }

        public bool IsQuotationPdf { get;set; }

        public bool IsDrivingLicensePdf { get; set; }

        public bool IsSeviceExtnPdf { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedOn { get; set; } = DateTime.Now;
    }
}
