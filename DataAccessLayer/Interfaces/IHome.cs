using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IHome
    {
        Task<List<DTOUserCountResponse>> GetUserCount();
        Task<List<DTOUserCountResponse>> GetApplicationCount(int userId);

        Task<List<DTOApprovedLogs>> GetApprovedLogs();
    }
}
