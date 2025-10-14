using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOHbaApplicationresponse
    {
        public string PropertyType { get; set; }

        public int PropertyTypeId { get; set; }

        public string? PropertySeller { get; set; }


        public string? PropertyAddress { get; set; }


        public decimal? PropertyCost { get; set; }


        public int? HBA_LoanFreq { get; set; }


        public decimal? HBA_repayingCapacity { get; set; }

        public decimal? HBA_Amt_Eligible_for_loan { get; set; }


        public decimal? HBA_EMI_Eligible { get; set; }


        public decimal? HBA_Amount_Applied_For_Loan { get; set; }


        public decimal? HBA_EMI_Applied { get; set; }


        public decimal? HBA_approxEMIAmount { get; set; }


        public decimal? HBA_approxDisbursementAmt { get; set; }
    }
}
