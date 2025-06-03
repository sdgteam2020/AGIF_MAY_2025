using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Request
{
    public class DTOOnlineApplication
    {
        public CarApplicationModel? CarApplication { get; set; }
        public PCAApplicationModel? PCAApplication { get; set; }
        public HBAApplicationModel? HBAApplication { get; set; }
        public CommonDataModel? CommonData { get; set; }

        public string? loantype { get; set; }

        public string? applicantCategory { get; set; }
    }
}
