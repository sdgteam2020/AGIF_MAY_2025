using DataTransferObject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTODashboardViewModelReponse
    {
        public SessionUserDTO SessionUser { get; set; }

        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }

        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
    }
}
