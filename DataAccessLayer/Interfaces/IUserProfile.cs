using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUserProfile : IGenericRepositoryDL<UserProfile>
    {
        Task<UserProfile> GetByUserName(string userName);
        IQueryable<DTOUserProfileResponse> GetAllUser(bool status);


        Task<DTOUserProfileResponse> GetUserAllDetails(string userName);

        Task<DTOUserProfileResponse> GetAdminDetails();

        Task<bool> SaveTrnFwdRecords(TrnFwd trnFwd);
        Task<bool> SaveApprovedLogs(int UserId, string Ip, bool isActive,int coProfileId);

        Task<bool> DeleteUserAsync(string domainId, int profileId);
        Task<bool> SaveLoginLogs(DTOLoginLogs loginLog);

    }
}
