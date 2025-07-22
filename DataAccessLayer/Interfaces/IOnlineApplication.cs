using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IOnlineApplication : IGenericRepositoryDL<CommonDataModel>
    {
        //Task<bool> IsUser(string AadharNo);
        Task<string> GetFormType(int ApplicationID);

        Task<DateTime> GetRetirementDate(int rankId, int Prefix,DateTime dateTime);
        Task<DTOCommonOnlineApplicationResponse> GetApplicationDetails(int applicationId);

        Task<CommonDataonlineResponse> GetApplicationDetailsByArmyNo(string armyNumber, string Prefix, string Suffix, int appType);

        Task<bool> DeleteExistingLoan(string armyNumber, string Prefix, string Suffix, int appType);

        Task<bool> UpdateApplicationStatus(int applicationId, int status);
        Task<bool> InsertStatusCounter(TrnStatusCounter trnStatusCounter);

        Task<bool> CheckForCoRegister(string ArmyNo);

        Task<bool> CheckIsUnitRegister(string ArmyNo);

        Task<bool> CheckIsCoRegister(int UnitId);
        Task<bool> AddFwdCO(TrnFwdCO trnFwdCO);

        Task<bool> UpdateMergePdfStatus(int applicationId, bool status);
        Task<UserMapping?> GetUserDetails(string CoArmyNumber);
        Task<DTOCommonOnlineApplicationResponse> GetUnitByApplicationId(int applicationId);

        Task<string?> GetIOArmyNoAsync(int applicationId);

        Task<DTOCommonOnlineApplicationResponseList> GetApplicationDetailsForExport(DTOExportRequest dTOExport);
        Task<bool> CheckDocumentUploaded(int ApplicationID);

        Task<string> GetCOName(int mappingId);

        Task<bool> CheckExtensionofservice(int applicationid);

        Task<DataTable> GetApplicationDetailsForExcel(DTOExportRequest dTOExport);

        Task<UserMapping?> GetCoDetails(int applicationId);
        Task<int> GetVehicleType(int applicationId, string formType);
    }
}
