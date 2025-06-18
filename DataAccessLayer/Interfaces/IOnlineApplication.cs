using DataTransferObject.Model;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IOnlineApplication : IGenericRepositoryDL<CommonDataModel>
    {
        //Task<bool> IsUser(string AadharNo);
        Task<DateTime> GetRetirementDate(int rankId, int Prefix,DateTime dateTime);
        Task<DTOCommonOnlineApplicationResponse> GetApplicationDetails(int applicationId);

        Task<CommonDataonlineResponse> GetApplicationDetailsByArmyNo(string armyNumber, string Prefix, string Suffix, int appType);

        Task<bool> DeleteExistingLoan(string armyNumber, string Prefix, string Suffix, int appType);

        Task<bool> UpdateApplicationStatus(int applicationId, int status);

        Task<bool> CheckForCoRegister(string ArmyNo);

        Task<bool> CheckIsUnitRegister(string ArmyNo, int UnitId);

        Task<bool> CheckIsCoRegister(int UnitId);
        Task<bool> AddFwdCO(TrnFwdCO trnFwdCO);



    }
}
