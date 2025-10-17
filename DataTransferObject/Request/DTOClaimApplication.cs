using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Request
{
    public class DTOClaimApplication
    {
        public EducationDetailsModel? EducationDetails { get; set; }
        public MarriagewardModel? Marriageward { get; set; }
        public PropertyRenovationModel? PropertyRenovation { get; set; }
        public SplWaiverModel? SplWaiver { get; set; }

        public ClaimCommonModel? ClaimCommonData { get; set; }

        public ClaimAccountDetailsModel? AccountDetails { get; set; }
        public ClaimAddressDetailsModel? AddressDetails { get; set; }

        public string? Category { get; set; }

        public string? Purpose { get; set; }

        public string? COArmyNo { get; set; }

    }
}
