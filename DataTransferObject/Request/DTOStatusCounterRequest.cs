using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Request
{
    public class DTOStatusCounterRequest
    {
        public int statusId {  get; set; }
        public int ApplicationId { get; set; }
        public DateTime? ActionOn { get; set; } = DateTime.Now;
    }
}
