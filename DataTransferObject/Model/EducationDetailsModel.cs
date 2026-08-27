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
    public class EducationDetailsModel 
    {
        [Key]
        public int EDId { get; set; }

        [Required(ErrorMessage = "Application ID is required.")]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public ClaimCommonModel? ClaimCommonModel { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Child Name must only contain alphabets and spaces.")]
        [StringLength(20, ErrorMessage = "Child Name can't be longer than 20 characters.")]
        public string ChildName { get; set; }

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
        public string CourseForWithdrawal { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name Of college can't be longer than 50 characters.")]
        public string CollegeInstitution { get; set; }


        [NotMapped]
        [Required]
        public IFormFile AttachBonafideLetter { get; set; }

        public string? AttachBonafideLetterPdf { get; set; } 

        public string? AttachPartIIOrderPdf { get; set; }

        [NotMapped]
        [Required]
        public IFormFile TotalExpenditureFile { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total Expenditure should be a positive number")]
        public double TotalExpenditure { get; set; }
        public string? TotalExpenditureFilePdf { get; set; }

        public bool IsAttachPartIIOrderPdf { get; set; } // Flag to check if Part-II Order is attached

        public bool IsAttachBonafideLetterPdf { get; set; } // Flag to check if Bonafide Letter is attached

        public bool IsTotalExpenditureFilePdf { get; set; }
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "datetime2")]
        public DateTime? UpdatedOn { get; set; } = DateTime.Now;
    }
}
