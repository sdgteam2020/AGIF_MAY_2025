using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IClaimOnlineApplication : IGenericRepositoryDL<ClaimCommonModel>
    {
        bool ValidateFileUpload(IFormFile file, out string errorMessage);
        Task<string> GetFormType(int ApplicationID);
       Task<bool> submitApplication(DTOClaimApplication model, string PurposeType, int ApplicationId);

        Task<bool> ProcessFileUploads(List<IFormFile> files, string PurposeType, int ApplicationId);

        Task<DTOClaimCommonOnlineResponse> GetApplicationDetails(int applicationId);
        Task<string?> GetIOArmyNoAsync(int applicationId);

        Task<bool> UpdateApplicationStatus(int applicationId, int status);

        Task<bool> AddFwdCO(TrnFwdCO trnFwdCO);

        Task<UserMapping?> GetUserDetails(string CoArmyNumber);

        Task<(string Name, string Mobile, string Armyno)> GetCODetails(int mappingId);
        Task<bool> UpdateMergePdfStatus(int applicationId, bool status);

        Task<bool> CheckExtensionofservice(int applicationid);

        Task<DTOClaimCommonOnlineApplicationResponseList> GetApplicationDetailsForExport(DTOExportRequest dTOExport);

        Task<DataTable> GetApplicationDetailsForExcel(DTOExportRequest dTOExport);

        Task<ClaimCommonDataOnlineResponse> GetApplicationDetailsByArmyNo(string armyNumber, string Prefix, string Suffix, int appType);

        Task<bool> DeleteExistingLoan(string armyNumber, string Prefix, string Suffix, int appType);

        Task<DTOClaimCommonOnlineResponse> GetUnitByApplicationId(int applicationId);

        Task<bool> InsertStatusCounter(TrnClaimStatusCounter trnStatusCounter);

        }
}
