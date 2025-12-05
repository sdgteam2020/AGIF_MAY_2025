using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Helpers
{
    public class CommonParameters
    {
       public string armyNumber { get; set; } = string.Empty;
       public string Prefix { get; set; } = string.Empty;
       public string Suffix { get; set; } = string.Empty;
       public int appType { get; set; }  
    }
}
