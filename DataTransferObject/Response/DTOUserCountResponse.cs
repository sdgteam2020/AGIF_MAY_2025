using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOUserCountResponse
    {
        public string Status { get; set; } // e.g., "Active", "Inactive"
        public int Count { get; set; }
    }
}
