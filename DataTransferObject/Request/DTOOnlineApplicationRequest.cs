using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Request
{
    public class DTOOnlineApplicationRequest
    {
        public OnlineApplications onlineApplications { get; set; }
        public Car Cars { get; set; }
    }
}
