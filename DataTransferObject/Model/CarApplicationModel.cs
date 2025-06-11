using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class CarApplicationModel:Common
    {

        [Key]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Application ID is required.")]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public CommonDataModel? CommonDataModels { get; set; }

        [Required(ErrorMessage = "Dealer Name is required.")]
        [StringLength(100, ErrorMessage = "Dealer Name can't be longer than 100 characters.")]
        public string? DealerName { get; set; }

        [Required(ErrorMessage = "Vehicle Loan Type is required.")]
        //[StringLength(50, ErrorMessage = "Loan Type can't be longer than 50 characters.")]
        public int Veh_Loan_Type { get; set; }

        [Required(ErrorMessage = "Company Name is required.")]
        [StringLength(100, ErrorMessage = "Company Name can't be longer than 100 characters.")]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Model Name is required.")]
        [StringLength(100, ErrorMessage = "Model Name can't be longer than 100 characters.")]
        public string? ModelName { get; set; }

        [Required(ErrorMessage = "Vehicle Cost is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for vehicle cost.")]
        public int? VehicleCost { get; set; }

        [Required(ErrorMessage = "Loan Frequency is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Loan Frequency must be a valid number.")]
        public int? CA_LoanFreq { get; set; }
        [ForeignKey("CA_LoanFreq")]
        public MLoanFreq? MLoanFreq { get; set; }

        [Required(ErrorMessage = "Eligible loan amount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Vehicle Cost must be a valid amount.")]
        public decimal? CA_Amt_Eligible_for_loan { get; set; }

        [Required(ErrorMessage = "EMI Eligible amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for EMI eligible amount.")]
        public decimal? CA_EMI_Eligible { get; set; }

        [Required(ErrorMessage = "Repaying Capacity is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for repaying capacity.")]
        public decimal? CA_repayingCapacity { get; set; }

        [Required(ErrorMessage = "Amount Applied for Loan is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for applied loan amount.")]
        public decimal? CA_Amount_Applied_For_Loan { get; set; }

        [Required(ErrorMessage = "EMI Applied is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "EMI Applied must be greater than 0.")]
        public decimal? CA_EMI_Applied { get; set; }

        [Required(ErrorMessage = "Approximate EMI Amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for approximate EMI amount.")]
        public decimal? CA_approxEMIAmount { get; set; }

        [Required(ErrorMessage = "Driving License Number is required.")]
        [StringLength(50, ErrorMessage = "Driving License Number can't be longer than 50 characters.")]
        public string? DrivingLicenseNo { get; set; }

        [Required(ErrorMessage = "Validity Date of Driving License is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid format for validity date.")]
        public string? Validity_Date_DL { get; set; }

        [Required(ErrorMessage = "Driving License Issuing Authority is required.")]
        [StringLength(100, ErrorMessage = "Driving License Issuing Authority can't be longer than 100 characters.")]
        public string? DL_IssuingAuth { get; set; }

        [Required(ErrorMessage = "Approximate Disbursement Amount is required.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid format for disbursement amount.")]
        public decimal? CA_approxDisbursementAmt { get; set; }
    }
}
