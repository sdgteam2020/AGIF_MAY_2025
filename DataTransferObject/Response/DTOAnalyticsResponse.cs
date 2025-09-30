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
    }
}
