using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IDefault
    {
        Task<List<DTOApplicationStatusResponse>> GetUserApplicationStatusByArmyNo(string armyNo);
        Task<List<DTOApplicationStatusResponse>> GetTimeLine(int applicationId);
        Task<List<DTOApplicationStatusResponse>> GetClaimTimeLine(int applicationId);
        Task<List<DTOApplicationStatusResponse>> GetClaimUserApplicationStatusByArmyNo(string armyNo);
    }
}
