using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class ClaimDocumentUpload
    {
        [Key]
        public int UploadId { get; set; }

        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public ClaimCommonModel? ClaimCommonModel { get; set; }
        public bool IsAttachBonafideLetterPdf { get; set; }
        public bool IsAttachPartIIOrderPdfEdu { get; set; }
        public bool IsAttachInvitationcardPdf { get; set; }
        public bool IsAttach_PartIIOrderPdfMarr { get; set; }
        public bool IsTotalExpenditureFilePdf { get; set; }
        public bool IsOtherReasonPdf { get; set; }
        public bool IsCancelledChequePdf { get; set; }
        public bool IsPaySlipPdf { get; set; }
        public bool IsSplWaiverPdf { get; set; }
        public bool IsSeviceExtnPdf { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedOn { get; set; } = DateTime.Now;

    }
}
