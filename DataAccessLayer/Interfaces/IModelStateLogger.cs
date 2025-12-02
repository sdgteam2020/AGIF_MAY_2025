using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DataAccessLayer.Interfaces
{
    public interface IModelStateLogger
    {
        Task LogModelStateError(ModelStateDictionary modelState, HttpContext httpContext);
    }
}
