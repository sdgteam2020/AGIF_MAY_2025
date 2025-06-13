using DataAccessLayer.Interfaces;

namespace Agif_V2.Helpers
{
    public class PdfGenerator
    {
        private readonly IOnlineApplication _usersApplications;
        public PdfGenerator(IOnlineApplication usersApplications)
        {
            _usersApplications = usersApplications;
        }
        public async Task<int> CreatePdfForOnlineApplication(int applicationId)
        {
            var data = await _usersApplications.GetApplicationDetails(applicationId,"");
            return 1;
        }
    }
}
