using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Helpers
{
    public class SessionUserDTO
    {
        public string ArmyNo { get; set; } = string.Empty;
        public int ProfileId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsCO { get; set; }
        public string Role { get; set; } = string.Empty;
        public int MappingId { get; set; }
        public int UserId { get; set; }
        public string DomainId { get; set; }
        public string RankName { get; set; }


    }
}
