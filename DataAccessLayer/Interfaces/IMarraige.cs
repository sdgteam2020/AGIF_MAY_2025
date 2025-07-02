using DataTransferObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IMarraige : IGenericRepositoryDL<MarriagewardModel>
    {
        Task<MarriagewardModel?> GetByApplicationId(int ApplicationId);

    }
}
