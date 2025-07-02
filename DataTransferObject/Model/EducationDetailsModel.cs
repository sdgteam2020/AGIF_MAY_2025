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
    public class EducationDetailsModel : Common
    {
        [Key]
        public int EDId { get; set; }

        [Required(ErrorMessage = "Application ID is required.")]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public ClaimCommonModel? ClaimCommonModel { get; set; }

        [Required]
        public string ChildName { get; set; }

        // Date of Birth of the Child
        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format.")]
        public DateTime? DateOfBirth { get; set; }

        // DO Part-II No
        [Required]
        [RegularExpression(@"^\d{3,8}$", ErrorMessage = "Invalid DO Part-II No")]
        public string DOPartIINo { get; set; }

        // DO Part-II Date
        [Required]
        public DateTime? DoPartIIDate { get; set; }

        // Attach Part-II Order
        [NotMapped]
        [Required]
        public IFormFile AttachPartIIOrder { get; set; }

        // Presently studying in (School or College)

        // Course/Class for Withdrawal Required
        [Required]
        public string CourseForWithdrawal { get; set; }

        // Name of College/Institution where studying
        [Required]
        public string CollegeInstitution { get; set; }

        // Attach Bonafide certificate/admission letter
        [NotMapped]
        [Required]
        public IFormFile AttachBonafideLetter { get; set; }

        public string? AttachBonafideLetterPdf { get; set; } 

        public string? AttachPartIIOrderPdf { get; set; } 

        // Total Expenditure
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total Expenditure should be a positive number")]
        public double TotalExpenditure { get; set; }

        public bool IsAttachPartIIOrderPdf { get; set; } // Flag to check if Part-II Order is attached

        public bool IsAttachBonafideLetterPdf { get; set; } // Flag to check if Bonafide Letter is attached
    }
}
