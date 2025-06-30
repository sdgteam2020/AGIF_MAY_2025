using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IUsersApplications
    {
        Task<List<DTOGetApplResponse>> GetUsersApplication(int MappingId,int status);

        Task<List<DTOGetApplResponse>> GetUsersApplicationForAdmin(int status);

        Task<List<DTOGetApplResponse>> GetApplicationByDate(DateTime date);
    }
}
