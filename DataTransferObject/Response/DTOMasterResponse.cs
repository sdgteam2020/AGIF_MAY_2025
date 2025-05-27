using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOMasterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RetirementAge { get; set; }
        public int UserType { get; set; }
        public string Pcda_Pao { get; set; } = string.Empty;

    }
}
