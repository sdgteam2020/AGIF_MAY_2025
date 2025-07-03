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
    public class ClaimDocumentUpload:Common
    {
        [Key]
        public int UploadId { get; set; }

        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public ClaimCommonModel? ClaimCommonModel { get; set; }

        public string? AttachBonafideLetterPdf { get; set; }
        public string? AttachPartIIOrderPdf { get; set; }
        public bool IsAttachBonafideLetterPdf { get; set; }
        public bool IsAttachPartIIOrderPdf { get; set; }


        public string? Attach_PartIIOrderPdf { get; set; }
        public string? AttachInvitationcardPdf { get; set; }
        public bool IsAttachInvitationcardPdf { get; set; }
        public bool IsAttach_PartIIOrderPdf { get; set; }



        public string? TotalExpenditureFile { get; set; }
        public bool IsTotalExpenditureFilePdf { get; set; }


        public string? OtherReasonsPdf { get; set; }
        public bool IsOtherReasonPdf { get; set; }

        public string? CancelledCheque { get; set; }
        public string? PaySlipPdf { get; set; }
        public string? SplWaiverPdf { get; set; }
        public bool IsCancelledChequePdf { get; set; }
        public bool IsPaySlipPdf { get; set; }
        public bool IsSplWaiverPdf { get; set; }
    }
}
