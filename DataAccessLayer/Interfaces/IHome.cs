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
        Task<List<DTOUserCountResponse>> GetClaimApplicationCount(int userId);

        Task<List<DTOApprovedLogs>> GetApprovedLogs();
        Task<DTOAnalyticsResult> GetTotalMonthlyApplications(int year);

        Task<DTOAnalyticsResult> GetTotalClaimMonthlyApplications(int year);
        Task AddVisitorAsync(string ipAddress);
        Task<int> GetTodayCountAsync();
        Task<int> GetMonthlyCountAsync();
        Task<int> GetTotalCountAsync();
    }
}
