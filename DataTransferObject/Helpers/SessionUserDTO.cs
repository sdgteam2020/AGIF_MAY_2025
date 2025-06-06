using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Helpers
{
    public class SessionUserDTO
    {
        public int ProfileId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsCO { get; set; }
        public string Role { get; set; } = string.Empty;

    }
}
