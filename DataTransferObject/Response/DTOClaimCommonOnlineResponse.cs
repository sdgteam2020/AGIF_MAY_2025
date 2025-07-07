using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{
    public class DTOClaimCommonOnlineResponse
    {
        public DTOEducationDetailsResponse? EducationDetailsResponse { get; set; }
        public DTOMarraigeWardResponse? MarraigeWardResponse { get; set; }
        public DTOPropertyRenovationResponse? PropertyRenovationResponse { get; set; }
        public ClaimCommonDataOnlineResponse? OnlineApplicationResponse { get; set; }
        public List<DTODocumentFileView>? Documents { get; set; }

    }
}
