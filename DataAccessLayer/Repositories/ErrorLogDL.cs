using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ErrorLogDL : GenericRepositoryDL<ErrorLog>, IErrorLog
    {
        protected new readonly ApplicationDbContext _context;
        public ErrorLogDL(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }

        public async Task AddError(ErrorLog errorLog)
        {
            _context.ErrorLogs.Add(errorLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogExceptionAsync(Exception exception, HttpContext httpContext)
        {
            try
            {
                string exceptionName = exception.GetType().Name;

                var exceptionType = await _context.MExceptionTypes
                    .FirstOrDefaultAsync(x => x.ExceptionTypeName == exceptionName);

                if (exceptionType == null)
                {
                    exceptionType = new MExceptionType
                    {
                        ExceptionTypeName = exceptionName
                    };

                    _context.MExceptionTypes.Add(exceptionType);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        exceptionType = await _context.MExceptionTypes
                            .FirstOrDefaultAsync(x => x.ExceptionTypeName == exceptionName);
                    }
                }

                if (exceptionType == null)
                {
                    return; // Don't let logging throw another exception
                }

                var errorLog = new ErrorLog
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ExceptionTypeId = exceptionType.ExceptionTypeId,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    Path = httpContext.Request.Path,
                    Created = DateTime.Now
                };

                _context.ErrorLogs.Add(errorLog);
                await _context.SaveChangesAsync();
            }
            catch
            {
                // Never allow logging failures to crash the application
            }
        }
    }
}
