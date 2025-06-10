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
    }
}
