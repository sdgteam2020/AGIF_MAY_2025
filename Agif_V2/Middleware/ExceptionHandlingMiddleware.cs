using Agif_V2.Models;
using DataAccessLayer;
using DataAccessLayer.Interfaces;
using DataTransferObject.Model;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Agif_V2.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate requestDelegate)
        {
            _logger = logger;
            _next = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext httpContext,ApplicationDbContext context, IErrorLog error)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                await HandleCustomExceptionResponseAsync(httpContext,ex, context,error);
            }
           
        }

        public async Task HandleCustomExceptionResponseAsync(HttpContext httpContext,Exception ex, ApplicationDbContext context, IErrorLog error)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

            ErrorLog errorLogs = new ErrorLog();
            errorLogs.StatusCode = (int)HttpStatusCode.InternalServerError;
            errorLogs.Message = ex.Message;
            errorLogs.StackTrace = ex.StackTrace;
            errorLogs.Path = httpContext.Request.Path;
            errorLogs.ExceptionType = ex.GetType().Name;

            await error.Add(errorLogs);

            var response = new ErrorModel(httpContext.Response.StatusCode,ex.Message,ex.GetType().Name.ToString(),ex.StackTrace?.ToString());
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);
            await httpContext.Response.WriteAsync(json);
        }
    }
}
