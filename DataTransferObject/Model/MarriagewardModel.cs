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
    public class MarriagewardModel : Common
    {

        [Key]
        public int MWId { get; set; }

        [Required(ErrorMessage = "Application ID is required.")]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public ClaimCommonModel? ClaimCommonModel { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Child Name must only contain alphabets and spaces.")]
        public string NameOfChild { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format.")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public string DOPartIINo { get; set; }

        [Required]
        public DateTime? DoPartIIDate { get; set; }

        [NotMapped]
        [Required]
        public IFormFile AttachPartIIOrder { get; set; }

        [Required]
        public int AgeOfWard { get; set; }

        [Required(ErrorMessage = "Date of Marraige is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format.")]
        public DateTime? DateofMarriage { get; set; }

        [NotMapped]
        [Required]
        public IFormFile AttachInvitationcard { get; set; }

        public string? AttachPartIIOrderPdf { get; set; }

        public string? AttachInvitationcardPdf { get; set; }
        public bool IsAttachPartIIOrderPdf { get; set; }

        public bool IsAttachInvitationcardPdf { get;set; }
        [Required]
        public string Gender { get; set; } = string.Empty;

    }
}
