using DataTransferObject.Model;
using DataTransferObject.Request;
using DataTransferObject.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IClaimOnlineApplication : IGenericRepositoryDL<ClaimCommonModel>
    {
        bool ValidateFileUpload(IFormFile file, out string errorMessage);

       Task<bool> submitApplication(DTOClaimApplication model, string PurposeType, int ApplicationId);

        Task<bool> ProcessFileUploads(List<IFormFile> files, string PurposeType, int ApplicationId);

        Task<DTOClaimCommonOnlineResponse> GetApplicationDetails(int applicationId);
        Task<string?> GetIOArmyNoAsync(int applicationId);

        Task<bool> UpdateApplicationStatus(int applicationId, int status);

        Task<bool> AddFwdCO(TrnFwdCO trnFwdCO);

        Task<UserMapping?> GetUserDetails(string CoArmyNumber);

        Task<string> GetCOName(int mappingId);

        Task<bool> UpdateMergePdfStatus(int applicationId, bool status);
    }
}
