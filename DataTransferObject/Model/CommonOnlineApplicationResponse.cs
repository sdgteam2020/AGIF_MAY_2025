using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Model
{
    public class CommonOnlineApplicationResponse
    {
       public DTOCarApplicationresponse? CarApplicationResponse { get; set; }
       public DTOHbaApplicationresponse? HbaApplicationResponse { get; set; }
       public OnlineApplicationResponse? OnlineApplicationResponse { get; set; }
    }
}
