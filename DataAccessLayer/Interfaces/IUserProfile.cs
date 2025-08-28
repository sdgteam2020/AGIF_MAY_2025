using DataTransferObject.Model;
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
        //Task<List<DTOUserProfileResponse>> GetAllUser(bool status);
        IQueryable<DTOUserProfileResponse> GetAllUser(bool status);


        Task<DTOUserProfileResponse> GetUserAllDetails(string userName);

        Task<DTOUserProfileResponse> GetAdminDetails();

        Task<bool> SaveTrnFwdRecords(TrnFwd trnFwd);
        Task<bool> SaveApprovedLogs(string DomainId, string Ip, bool isActive,string coDomainId,int coProfileId);
    }
}
