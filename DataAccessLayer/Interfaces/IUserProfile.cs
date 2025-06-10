using DataTransferObject.Model;
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
        Task<UserProfile> GetAllUser(bool status);
    }
}
