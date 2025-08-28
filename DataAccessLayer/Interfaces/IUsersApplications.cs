using DataTransferObject.Helpers;
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
    public interface IUsersApplications
    {
        Task<List<DTOGetApplResponse>> GetUsersApplication(int Mapping, int status);

        Task<List<DTOGetApplResponse>> GetUsersApplicationForAdmin(int status);

        Task<List<DTOGetApplResponse>> GetApplicationByDate(DateTime date);

        Task<bool> UpdateStatus(DTOExportRequest dtoExport);

        Task<List<DTOGetApplResponse>> GetMaturityUsersApplication(int Mapping, int status);

        Task<List<DTOGetApplResponse>> GetClaimUsersApplicationForAdmin(int status);

        Task<bool> UpdateClaimStatus(DTOExportRequest dtoExport);

        Task<List<DTOGetApplResponse>> GetClaimApplicationByDate(DateTime date);

        Task<bool> UpdateUserDetails(SessionUserDTO sessionUserDTO);

        Task<bool> CheckAllApplnIdPresent(List<string> applicationIds);

        Task<List<string>> GetAllStatusCode();
        Task<List<string>> GetNotApplId(List<string> applicationIds);

        Task<(bool, string)> ProcessBulkApplicationUpdates(DataTable applicationUpdates);
        DataTable CreateApplicationUpdatesDataTable(List<DTOApplStatusBulkUpload> applications);
        Task<List<string>> GetAllClaimStatusCode();
        Task<List<string>> GetClaimNotApplId(List<string> applicationIds);

        Task<(bool, string)> ClaimProcessBulkApplicationUpdates(System.Data.DataTable applicationUpdates);
    }
}
