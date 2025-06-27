using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOCommonOnlineApplicationResponse
    {
        public DTOCarApplicationresponse? CarApplicationResponse { get; set; }
        public DTOHbaApplicationresponse? HbaApplicationResponse { get; set; }
        public DTOPCAApplicationresponse? PcaApplicationResponse { get; set; }
        public CommonDataonlineResponse? OnlineApplicationResponse { get; set; }

        public List<DTODocumentFileView>? Documents { get; set; }
    }
    public class DTOCommonOnlineApplicationResponseList
    {
        public List<DTOCarApplicationresponse>? CarApplicationResponse { get; set; }
        public List<DTOHbaApplicationresponse>? HbaApplicationResponse { get; set; }
        public List<DTOPCAApplicationresponse>? PcaApplicationResponse { get; set; }
        public List<CommonDataonlineResponse>? OnlineApplicationResponse { get; set; }

        public List<DTODocumentFileView>? Documents { get; set; }
    }
}
