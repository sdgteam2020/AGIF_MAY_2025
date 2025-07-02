using DataTransferObject.Model;
using DataTransferObject.Request;
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
    }
}
