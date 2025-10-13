using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAnalyticsResponse
    {
        public int TotalApplications { get; set; }
        public string Month { get; set; } = string.Empty;
        public int CACount { get; set; }
        public int PCACount { get; set; }
        public int HBACount { get; set; }
        public int RankCount { get; set; }
        public string Rank { get; set; } = string.Empty;
        public string Regt { get; set; } = string.Empty;
        public int RegtCount { get; set; }
        public string VehLoanType { get; set; } = string.Empty;
        public string LoanType { get; set; } = string.Empty;
        public int LoanCount { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public decimal TotalHbaLoan { get; set; }
        public decimal TotalCarLoan { get; set; }
        public decimal TotalPcaLoan { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public string DealerName { get; set; } = string.Empty;

        public string ApplicantName { get; set; }=string.Empty;

        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public string AgeGroup { get; set; } = string.Empty;
        public List<DateTime?> LoanDates { get; set; }  // List of loan dates (CA, PCA, HBA)

    }
}
