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
        
        public DTOSplWaiverResponse? SplWaiverResponse { get; set; }
        public ClaimCommonDataOnlineResponse? OnlineApplicationResponse { get; set; }

        public List<DTODocumentFileView>? Documents { get; set; }

    }

    public class DTOClaimCommonOnlineApplicationResponseList
    {
        public List<DTOEducationDetailsResponse>? EducationDetailsResponse { get; set; }
        public List<DTOMarraigeWardResponse>? MarraigeWardResponse { get; set; }
        public List<DTOPropertyRenovationResponse>? PropertyRenovationResponse { get; set; }
        public List<ClaimCommonDataOnlineResponse>? OnlineApplicationResponse { get; set; }

        public List<DTODocumentFileView>? Documents { get; set; }
    }
}
