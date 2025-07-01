using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Request
{
    public class DTOExportRequest
    {
        public required List<int> Id { get; set; }
    }
}
