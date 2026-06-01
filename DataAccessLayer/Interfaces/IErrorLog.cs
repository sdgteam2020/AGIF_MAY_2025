using DataTransferObject.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interfaces
{
    public interface IErrorLog :IGenericRepositoryDL<ErrorLog>
    {
        Task AddError(ErrorLog errorLog);

        Task LogExceptionAsync(Exception exception,HttpContext httpContext);
    }
}
