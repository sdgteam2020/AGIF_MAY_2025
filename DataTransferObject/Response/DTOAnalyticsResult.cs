using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOAnalyticsResult
    {
        public List<DTOAnalyticsResponse> MonthlyApplications { get; set; }
        public List<DTOAnalyticsResponse> TopRanks { get; set; }
        public List<DTOAnalyticsResponse> TopRegiments { get; set; }
        public List<DTOAnalyticsResponse> loanStats { get; set; }
        public List<DTOAnalyticsResponse> topUnits { get; set; }
        public List<DTOAnalyticsResponse> topUnitsByLoanAmount { get; set; }  
        public List<DTOAnalyticsResponse> topDealers { get; set; }  
        public List<DTOAnalyticsResponse> topLoanDealers { get; set; }  

    }
}
