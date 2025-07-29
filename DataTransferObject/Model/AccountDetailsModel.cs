using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class AccountDetailsModel:Common
    {

        [Key]
        public int AccountId { get; set; }
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public CommonDataModel? CommonDataModels { get; set; }

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
    }
}
