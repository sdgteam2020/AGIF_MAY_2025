using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOGetApplResponse
    {
        public int ApplicationId { get; set; }
        public string ArmyNo { get; set; }
        public string Name { get; set; }
        public int ApplicationType { get; set; }
        public string DateOfBirth { get; set; }
        public string PresentStatus { get; set; }
        
    }
}
