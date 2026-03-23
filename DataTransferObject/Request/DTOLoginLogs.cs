using System;
using System.Collections.Generic;
using System.Text;

namespace DataTransferObject.Request
{
    public class DTOLoginLogs
    {
        public int UserId { get; set; }
        public int ProfileId { get; set; }
        public int RoleId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public DateTime LoginOn { get; set; }
    }
}
