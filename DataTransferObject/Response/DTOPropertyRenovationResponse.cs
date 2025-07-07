using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOPropertyRenovationResponse
    {
        public string AddressOfProperty { get; set; }
        public string PropertyHolderName { get; set; }
        public double EstimatedCost { get; set; }

    }
}
