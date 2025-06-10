using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class HBAApplicationModel:Common
    {
        [Key]
        public int HbaId {  get; set; }

        [Required(ErrorMessage = "Application ID is required.")]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public CommonDataModel? CommonDataModels { get; set; }

        [Required(ErrorMessage = "Property Type is required.")]
        public int PropertyType { get; set; }

        public string? PropertySeller { get; set; }

        [Required(ErrorMessage = "Property Address is required.")]
        [StringLength(300, ErrorMessage = "Property Address can't be longer than 300 characters.")]
        public string? PropertyAddress { get; set; }

        [Required(ErrorMessage = "Property Cost is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for property cost.")]
        public decimal? PropertyCost { get; set; }

        [Required(ErrorMessage = "Loan Frequency is required.")]
        [StringLength(50, ErrorMessage = "Loan Frequency can't be longer than 50 characters.")]
        public string? HBA_LoanFreq { get; set; }

        [Required(ErrorMessage = "Repaying Capacity is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for repaying capacity.")]
        public decimal? HBA_repayingCapacity { get; set; }

        [Required(ErrorMessage = "Eligible Loan Amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for eligible loan amount.")]
        public decimal? HBA_Amt_Eligible_for_loan { get; set; }

        [Required(ErrorMessage = "EMI Eligible amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for EMI eligible amount.")]
        public decimal? HBA_EMI_Eligible { get; set; }

        [Required(ErrorMessage = "Amount Applied for Loan is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for applied loan amount.")]
        public decimal? HBA_Amount_Applied_For_Loan { get; set; }

        [Required(ErrorMessage = "EMI Applied is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "EMI Applied must be greater than 0.")]
        public decimal? HBA_EMI_Applied { get; set; }

        [Required(ErrorMessage = "Approximate EMI Amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for approximate EMI amount.")]
        public decimal? HBA_approxEMIAmount { get; set; }

        [Required(ErrorMessage = "Approximate Disbursement Amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for disbursement amount.")]
        public decimal? HBA_approxDisbursementAmt { get; set; }
    }
}
