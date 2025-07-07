using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    //public class DTOApplicationStatusResponse
    //{
    //    public LoanStatus LoanType1 { get; set; } = new LoanStatus();
    //    public LoanStatus LoanType2 { get; set; } = new LoanStatus();
    //    public LoanStatus LoanType3 { get; set; } = new LoanStatus();
    //}

    //public class LoanStatus
    //{
    //    public string LoanType { get; set; }
    //    public string StatusCode { get; set; }
    //}

    public class DTOApplicationStatusResponse
    {
        public int ApplicationId { get; set; }
        public string ApplicationType { get; set; }
        public string Status { get; set; }

        public string timeLine { get; set; }

        public int StatusId { get; set; }

    }
}
