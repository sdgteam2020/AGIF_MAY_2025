using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class ClaimCommonModel : Common
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
        //[EmailAddress(ErrorMessage = "Invalid Email Address.")]
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

        [Required(ErrorMessage = "Village/Town is required.")]
        [StringLength(100, ErrorMessage = "Village/Town can't be longer than 100 characters.")]
        public string? Vill_Town { get; set; }

        [Required(ErrorMessage = "Post Office is required.")]
        [StringLength(100, ErrorMessage = "Post Office can't be longer than 100 characters.")]
        public string? PostOffice { get; set; }

        [Required(ErrorMessage = "District is required.")]
        [StringLength(100, ErrorMessage = "District can't be longer than 100 characters.")]
        public string? Distt { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State can't be longer than 100 characters.")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Zip Code is required.")]
        [StringLength(6, ErrorMessage = "Zip Code can't be longer than 6 characters.")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Salary Account No is required.")]
        [StringLength(20, ErrorMessage = "Salary Account No can't be longer than 20 characters.")]
        public string? SalaryAcctNo { get; set; }

        [Required(ErrorMessage = "Re-Enter Salary Account No is required.")]
        [StringLength(20, ErrorMessage = "Confirm Salary Account No can't be longer than 20 characters.")]
        public string? ConfirmSalaryAcctNo { get; set; }

        [Required(ErrorMessage = "IFSC Code is required.")]
        [StringLength(11, ErrorMessage = "IFSC Code can't be longer than 11 characters.")]
        public string? IfsCode { get; set; }

        [Required(ErrorMessage = "Bank Name is required.")]
        [StringLength(100, ErrorMessage = "Bank Name can't be longer than 100 characters.")]
        public string? NameOfBank { get; set; }

        [Required(ErrorMessage = "Bank Branch is required.")]
        [StringLength(100, ErrorMessage = "Bank Branch Name can't be longer than 100 characters.")]
        public string? NameOfBankBranch { get; set; }


        public bool? House_Building_Advance_Loan { get; set; }
        public bool? House_Repair_Advance_Loan { get; set; }
        public bool? Conveyance_Advance_Loan { get; set; }
        public bool? Computer_Advance_Loan { get; set; }



        public DateTime? House_Building_Date_of_Loan_taken { get; set; }
        public int? House_Building_Duration_of_Loan { get; set; }
        public decimal? House_Building_Amount_Taken { get; set; }


        public DateTime? House_Repair_Advance_Date_of_Loan_taken { get; set; }
        public int? House_Repair_Advance_Duration_of_Loan { get; set; } 
        public decimal? House_Repair_Advance_Amount_Taken { get; set; }


        public DateTime? Conveyance_Date_of_Loan_taken { get; set; }
        public int? Conveyance_Duration_of_Loan { get; set; } 
        public decimal? Conveyance_Amount_Taken { get; set; }


        public DateTime? Computer_Date_of_Loan_taken { get; set; }
        public int? Computer_Duration_of_Loan { get; set; } 
        public decimal? Computer_Amount_Taken { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid estimated cost.")]
        public double? AmountOfWithdrawalRequired { get; set; } 

        public string? Noofwithdrawal { get; set; }


        [Required]
        public int ApplicantType { get; set; }
        [ForeignKey("ApplicantType")]
        public MApplicantType? MApplicantType { get; set; }

        [Required]
        public int WithdrawPurpose { get; set; }
        [ForeignKey("WithdrawPurpose")]
        public WithdrawalPurpose? WithdrawalPurposetype { get; set; }

        public int StatusCode { get; set; }

        public bool IsMergePdf { get; set; } = false;
        public string? IOArmyNo { get; set; }

        public DateTime? DownloadedOn { get; set; }
        public int DownloadCount { get; set; }
    }
}
