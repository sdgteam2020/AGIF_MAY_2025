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

        public async Task InvokeAsync(
            HttpContext httpContext,
            IErrorLog errorLog)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await errorLog.LogExceptionAsync(ex, httpContext);

                await HandleCustomExceptionResponseAsync(
                    httpContext);
            }
        }

        private async Task HandleCustomExceptionResponseAsync(
            HttpContext httpContext)
        {
            httpContext.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            httpContext.Response.ContentType =
                MediaTypeNames.Application.Json;

            var response = new ErrorModel(
                httpContext.Response.StatusCode,
                "An unexpected error occurred processing your request.",
                null);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            };

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(response, options));
        }

    }
}
