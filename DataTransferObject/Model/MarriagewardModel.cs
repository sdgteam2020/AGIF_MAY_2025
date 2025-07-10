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
        public string NameOfChild { get; set; }

        // Date of Birth of the Child
        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format.")]
        public DateTime? DateOfBirth { get; set; }

        // DO Part-II No
        [Required]
        //[RegularExpression(@"^\d{3,8}$", ErrorMessage = "Invalid DO Part-II No")]
        public string DOPartIINo { get; set; }

        // DO Part-II Date
        [Required]
        public DateTime? DoPartIIDate { get; set; }

        // Attach Part-II Order
        [NotMapped]
        [Required]
        public IFormFile AttachPartIIOrder { get; set; }

        // Presently studying in (School or College)
        [Required]
        public int AgeOfWard { get; set; }

        // Course/Class for Withdrawal Required
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

    }
}
