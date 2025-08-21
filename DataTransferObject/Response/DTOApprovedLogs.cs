using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOApprovedLogs
    {
        public string DomainId { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string CoDomainId { get; set; }
        public int CoProfileId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsApproved { get; set; }
    }
}
