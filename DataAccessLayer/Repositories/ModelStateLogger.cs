using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using iText.Bouncycastle.Crypto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ModelStateLogger : IModelStateLogger
    {
        private readonly ApplicationDbContext _context;
        public ModelStateLogger(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task LogModelStateError(ModelStateDictionary modelState, HttpContext httpContext)
        {
            if(modelState== null || modelState.IsValid)
            {
                return;
            }
            // Extract all ModelState errors
            var allErrors = modelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .Select(ms => $"{ms.Key} : {string.Join(", ", ms.Value.Errors.Select(e => e.ErrorMessage))}")
                .ToList();

            // Combine message
            string message = string.Join(" | ", allErrors);

            // Log the error to the database
            ErrorLog errorLog = new ErrorLog
            {
                StatusCode = 400, // Bad Request
                ExceptionType = "ModelStateValidationError",
                Message = message,
                StackTrace = null,
                Path = httpContext.Request.Path,
                Created = DateTime.Now
            };
            _context.ErrorLogs.Add(errorLog);
            await _context.SaveChangesAsync();
        }
    }
}
