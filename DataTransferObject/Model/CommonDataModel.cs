using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class CommonDataModel:Common
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Army Prefix is required.")]
       
        public int ArmyPrefix { get; set; }
        [ForeignKey("ArmyPrefix")]
        public MArmyPrefix? MArmyPrefix { get; set; }

        [Required(ErrorMessage = "Number is required.")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Number must only contain alphanumeric characters.")]

        public string? Number { get; set; }

        [Required(ErrorMessage = "Suffix is required.")]
        [StringLength(5, ErrorMessage = "Suffix can't be longer than 5 characters.")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Suffix must only contain alphanumeric characters.")]

        public string? Suffix { get; set; }

        [Required(ErrorMessage = "Old Army Prefix is required.")]
        public int OldArmyPrefix { get; set; }

        [Required(ErrorMessage = "Old Number is required.")]
        [StringLength(10, ErrorMessage = "Army Prefix can't be longer than 10 characters.")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Old Number must only contain alphanumeric characters.")]
        public string? OldNumber { get; set; }

        [Required(ErrorMessage = "Old Suffix is required.")]
        [StringLength(10, ErrorMessage = "Old Suffix can't be longer than 10 characters.")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Old Suffix must only contain alphanumeric characters.")]
        public string? OldSuffix { get; set; }

        [Required(ErrorMessage = "Rank is required.")]
        public int DdlRank { get; set; }
        [ForeignKey("DdlRank")]
        public MRank? MRank { get; set; }

        [Required(ErrorMessage = "Applicant Name is required.")]
        [StringLength(200, ErrorMessage = "Applicant Name can't be longer than 200 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Applicant Name must only contain alphabets and spaces.")]
        public string? ApplicantName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format.")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Date of Commission is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format.")]
        public DateTime? DateOfCommission { get; set; }

        public string? ExtnOfService { get; set; }

        public DateTime? DateOfPromotion { get; set; }

        public DateTime? DateOfRetirement { get; set; }

        [Required(ErrorMessage = "Aadhar Card No is required.")]
        public string? AadharCardNo { get; set; }

        [Required(ErrorMessage = "PAN Card No is required.")]
        public string? PanCardNo { get; set; }

        [Required(ErrorMessage = "Mobile No is required.")]
        [StringLength(10, ErrorMessage = "Mobile No must be 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Mobile No.")]
        public string? MobileNo { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Email Domain is required.")]
        [StringLength(100, ErrorMessage = "Email Domain can't be longer than 100 characters.")]
        public string? EmailDomain { get; set; }

        public int? TotalService { get; set; }

        public int? ResidualService { get; set; }

        [Required(ErrorMessage = "Regt/Corps is required.")]
        public int RegtCorps { get; set; }
        [ForeignKey("RegtCorps")]
        public MRegtCorps? MRegtCorps { get; set; }

        [StringLength(200, ErrorMessage = "PCDA PAO can't be longer than 200 characters.")]
        public string? pcda_pao { get; set; }

        [StringLength(20, ErrorMessage = "PCDA Account No can't be longer than 20 characters.")]
        public string? pcda_AcctNo { get; set; }

        [Required(ErrorMessage = "Parent Unit is required.")]
        public int ParentUnit { get; set; }
        [ForeignKey("ParentUnit")]
        public MUnit? MUnitsParent { get; set; }
        
        [Required(ErrorMessage = "Present Unit is required.")]
        public int PresentUnit { get; set; }
        [ForeignKey("PresentUnit")]
        public MUnit? MUnitsPresent { get; set; }


        [Required(ErrorMessage = "Present Unit Pin is required.")]
        [StringLength(6, ErrorMessage = "Present Unit Pin can't be longer than 6 characters.")]
        public string? PresentUnitPin { get; set; }

        [Required(ErrorMessage = "Army Post Office is required.")]
        public int ArmyPostOffice { get; set; }
        [ForeignKey("ArmyPostOffice")]
        public MArmyPostOffice? MArmyPostOffices { get; set; }

        public string? CivilPostalAddress { get; set; }

        [Required(ErrorMessage = "Next Fmn HQ is required.")]
        [StringLength(100, ErrorMessage = "Next Fmn HQ can't be longer than 100 characters.")]
        public string? NextFmnHQ { get; set; }

        // Financial information

        [Required(ErrorMessage = "Monthly Pay Slip is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format.")]

        public DateTime? MonthlyPaySlip { get; set; }

        [Required(ErrorMessage = "BasicPay is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Basic Pay must be a positive number.")]

        public decimal? BasicPay { get; set; }

        [Required(ErrorMessage = "DSOP/AFPP is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "DSOP/AFPP must be a positive number.")]
        public decimal? dsop_afpp { get; set; }

        public decimal? rank_gradePay { get; set; }

        [Required(ErrorMessage = "AGIF Subs is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "AGIF Subs must be a positive number.")]
        public decimal? agif_Subs { get; set; }

        [Required(ErrorMessage = "MSP is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "MSP must be a positive number.")]
        public decimal? Msp { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Income Tax Monthly must be a positive number.")]
        public decimal? IncomeTaxMonthly { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "CI Pay must be a positive number.")]
        public decimal? CI_Pay { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Education Cess must be a positive number.")]
        public decimal? EducationCess { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "NPA/X Gp Pay must be a positive number.")]
        public decimal? npax_Pay { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "PLI must be a positive number.")]
        public decimal? Pli { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tech Pay must be a positive number.")]
        public string? TechPay { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Misc Deduction must be a positive number.")]
        public decimal? misc_Deduction { get; set; }


        [Required(ErrorMessage = "DA is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "DA must be a positive number.")]
        public decimal? Da { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Loan EMI Outside must be a positive number.")]
        public decimal? loanEMI_Outside { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "PMHA must be a positive number.")]
        public decimal? Pmha { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Loan EMI must be a positive number.")]
        public decimal? LoanEmi { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "LRA must be a positive number.")]
        public decimal? Lra { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Misc Pay must be a positive number.")]
        public decimal? MiscPay { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total Credit must be a positive number.")]
        public decimal? TotalCredit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total Deductions must be a positive number.")]
        public decimal? TotalDeductions { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary After Deductions must be a positive number.")]
        public decimal? salary_After_Deductions { get; set; }

        [Required]
        public int ApplicationType { get; set; }
        [ForeignKey("ApplicationType")]
        public MApplicationType? MApplicationTypes { get; set; }

        [Required]
        public int ApplicantType { get ; set; }
        [ForeignKey("ApplicantType")]
        public MApplicantType? MApplicantType { get; set; }
        public int StatusCode { get; set; }
        public bool IsMergePdf { get; set; } = false;
        public string? IOArmyNo { get; set; }
        public DateTime? DownloadedOn { get; set; }
        public int  DownloadCount { get; set; }

        public string? AGIFRemarks { get; set; }= string.Empty;
    }
}
