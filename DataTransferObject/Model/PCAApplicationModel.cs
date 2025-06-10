using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class PCAApplicationModel:Common
    {
        [Key]
        public int PcaId {  get; set; }

        [Required(ErrorMessage = "Application ID is required.")]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public CommonDataModel? CommonDataModels { get; set; }

        [Required(ErrorMessage = "Dealer Name is required.")]
        [StringLength(100, ErrorMessage = "Dealer Name can't be longer than 100 characters.")]
        public string? PCA_dealerName { get; set; }

        [Required(ErrorMessage = "Loan Type is required.")]
        public int computer_Loan_Type { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(100, ErrorMessage = "Company Name can't be longer than 100 characters.")]
        public string? PCA_companyName { get; set; }

        [Required(ErrorMessage = "Model Name is required.")]
        [StringLength(100, ErrorMessage = "Model Name can't be longer than 100 characters.")]
        public string? PCA_modelName { get; set; }

        [Required(ErrorMessage = "Computer Cost is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for computer cost.")]
        public decimal? computerCost { get; set; }

        [Required(ErrorMessage = "Loan Frequency is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Loan Frequency must be a valid number.")]
        public string? PCA_LoanFreq { get; set; }

        [Required(ErrorMessage = "Eligible loan amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for eligible loan amount.")]
        public decimal? PCA_Amt_Eligible_for_loan { get; set; }

        [Required(ErrorMessage = "EMI Eligible amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for EMI eligible amount.")]
        public decimal? PCA_EMI_Eligible { get; set; }

        [Required(ErrorMessage = "Repaying Capacity is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for repaying capacity.")]
        public decimal? PCA_repayingCapacity { get; set; }

        [Required(ErrorMessage = "Amount Applied for Loan is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for applied loan amount.")]
        public decimal? PCA_Amount_Applied_For_Loan { get; set; }

        [Required(ErrorMessage = "EMI Applied is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "EMI Applied must be greater than 0.")]
        public decimal? PCA_EMI_Applied { get; set; }

        [Required(ErrorMessage = "Approximate EMI Amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for approximate EMI amount.")]
        public decimal? PCA_approxEMIAmount { get; set; }

        [Required(ErrorMessage = "Approximate Disbursement Amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for approximate disbursement amount.")]
        public decimal? PCA_approxDisbursementAmt { get; set; }

        public int? totalResidualMonth { get; set; }
    }
}
