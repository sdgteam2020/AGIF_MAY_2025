using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{

    public class DTOApplicationStatusResponse
    {
        public int ApplicationId { get; set; }
        public string ApplicationType { get; set; }
        public string Status { get; set; }

        public string timeLine { get; set; }

        public int StatusId { get; set; }

    }
}
