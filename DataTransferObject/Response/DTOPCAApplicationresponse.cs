using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOPCAApplicationresponse
    {
        public string? PCA_dealerName { get; set; }
        public string computer_Loan_Type { get; set; }
        public string? PCA_companyName { get; set; }
        public string? PCA_modelName { get; set; }

        public decimal? computerCost { get; set; }
        public int? PCA_LoanFreq { get; set; }

        public decimal? PCA_Amt_Eligible_for_loan { get; set; }
        public decimal? PCA_EMI_Eligible { get; set; }
        public decimal? PCA_repayingCapacity { get; set; }

        public decimal? PCA_Amount_Applied_For_Loan { get; set; }

        public decimal? PCA_EMI_Applied { get; set; }

        public decimal? PCA_approxEMIAmount { get; set; }
        public decimal? PCA_approxDisbursementAmt { get; set; }
    }
}
