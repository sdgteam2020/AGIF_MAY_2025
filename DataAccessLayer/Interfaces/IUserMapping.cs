using DataAccessLayer.Repositories;
using DataTransferObject.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUserMapping : IGenericRepositoryDL<UserMapping>
    {
        Task<UserMapping> GetUnitDetails(int applicationId);

        Task<UserMapping> GetByUserName(string userName);
        Task<List<UserMapping>> GetAllUser(bool status);
        Task<List<UserMapping>> GetByProfileId(int profileId);
        Task<List<UserMapping>> GetByUserId(int userId);
        Task<List<UserMapping>> GetByUnitId(int userId);
        Task<List<UserMapping>> GetByProfileIdAndApplicationId(int profileId, int applicationId);
        Task<List<UserMapping>> GetByProfileIdAndStatus(int profileId, int status);
    }
}
