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

        public string? ProfileName { get; set; }
        public string? AppointmentName { get; set; }
        public string? MobileNo { get; set; }
        public string? RegtName { get; set; }
        public string? UnitName { get; set; }
        public string? EmailId { get; set; }
        public bool IsPrimary { get; set; }
        public bool DteFmn { get; set; }
        public bool IsActive { get; set; }
        public bool IsCOActive { get; set; }
        public int RankId { get; set; }
        public int RegtId { get; set; }

        public int ApptId { get; set; }

        public int UnitId { get; set; }

        public string? name { get; set; } = string.Empty;
        public string Nameid { get; set; }
        public string AppName { get; set; }
    }
}
