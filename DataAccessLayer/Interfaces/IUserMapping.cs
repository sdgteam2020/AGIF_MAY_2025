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

    }
}
