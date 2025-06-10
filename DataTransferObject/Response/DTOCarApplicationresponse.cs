using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCarApplicationresponse
    {

        public string? DealerName { get; set; }
        public string Veh_Loan_Type { get; set; }
        public string? CompanyName { get; set; }
        public string? ModelName { get; set; }
        public int? VehicleCost { get; set; }
        public int? CA_LoanFreq { get; set; }
        public decimal? CA_Amt_Eligible_for_loan { get; set; }
        public decimal? CA_EMI_Eligible { get; set; }
        public decimal? CA_repayingCapacity { get; set; }
        public decimal? CA_Amount_Applied_For_Loan { get; set; }
        public decimal? CA_EMI_Applied { get; set; }
        public decimal? CA_approxEMIAmount { get; set; }
        public string? DrivingLicenseNo { get; set; }
        public string? Validity_Date_DL { get; set; }
        public string? DL_IssuingAuth { get; set; }
        public decimal? CA_approxDisbursementAmt { get; set; }
    }
}
